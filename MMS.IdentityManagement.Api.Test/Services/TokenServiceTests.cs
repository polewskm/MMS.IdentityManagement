using System;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.Options;
using MMS.IdentityManagement.Api.Services;
using MMS.IdentityManagement.Claims;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public sealed class TokenServiceTests : IDisposable
    {
        private const string TestIssuer = "test_iss";
        private const string TestIdentityProvider = "test_idp";

        private readonly DateTimeOffset _now = DateTimeOffset.Now;

        private readonly X509Certificate2 _certificate;
        private readonly CreateTokenRequest _createTokenRequest;

        private readonly TokenOptions _tokenOptions;
        private readonly ITokenService _tokenService;

        public TokenServiceTests()
        {
            _certificate = CreateSelfSignedCertificate();

            _createTokenRequest = new CreateTokenRequest
            {
                Client = new Client
                {
                    Id = "test_client",
                    RequireSecret = true,
                    Disabled = false,
                    Secrets = new[]
                    {
                        new Secret
                        {
                            Id = "test_secret",
                            CipherType = "test_cipher",
                            CipherText = "test_keycode",
                            CreatedWhen = _now,
                            UpdatedWhen = _now,
                        },
                    },
                },

                Member = new Member
                {
                    MemberId = 1,

                    DisplayName = Guid.NewGuid().ToString("N"),
                    FirstName = Guid.NewGuid().ToString("N"),
                    LastName = Guid.NewGuid().ToString("N"),
                    EmailAddress = Guid.NewGuid().ToString("N"),
                    PhoneNumber = Guid.NewGuid().ToString("N"),

                    MemberSince = _now.AddMonths(-1),
                    RenewalDue = _now.AddMonths(1),

                    BoardMemberType = BoardMemberType.MemberAtLarge,

                    Roles = new[]
                    {
                        "test_role1",
                        "test_role2",
                        "test_role3",
                    },
                    ChampionAreas = new[]
                    {
                        "test_area1",
                        "test_area2",
                        "test_area3",
                    }
                },

                AuthenticationType = Guid.NewGuid().ToString("N"),
                AuthenticationTime = _now,
                Nonce = Guid.NewGuid().ToString("N"),
            };

            var mockSystemClock = new Mock<ISystemClock>(MockBehavior.Strict);
            mockSystemClock
                .Setup(_ => _.UtcNow)
                .Returns(_now)
                .Verifiable();

            _tokenOptions = new TokenOptions
            {
                Issuer = TestIssuer,
                IdentityProvider = TestIdentityProvider,
                AccessTokenLifetime = TimeSpan.FromMinutes(2.0),
                RefreshTokenLifetime = TimeSpan.FromMinutes(3.0),
                SigningCredentials = new X509SigningCredentials(_certificate)
            };
            var tokenOptionsAccessor = Microsoft.Extensions.Options.Options.Create(_tokenOptions);

            _tokenService = new TokenService(tokenOptionsAccessor, mockSystemClock.Object);
        }

        public void Dispose()
        {
            _certificate.Dispose();
        }

        private static X509Certificate2 CreateSelfSignedCertificate()
        {
            const string certificateName = "test.localhost";

            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName(Environment.MachineName);
            sanBuilder.AddDnsName("localhost");

            var distinguishedName = new X500DistinguishedName($"CN={certificateName}");

            using (var rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.DigitalSignature,
                    false));

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") },
                        false));

                request.CertificateExtensions.Add(sanBuilder.Build());

                var now = DateTimeOffset.Now;
                var certificate = request.CreateSelfSigned(
                    now.AddDays(-1),
                    now.AddDays(1));

                certificate.FriendlyName = certificateName;

                return certificate;
            }
        }

        private void AssertIdentity(ClaimsIdentity identity)
        {
            Assert.NotNull(identity);

            var member = _createTokenRequest.Member;

            Assert.Equal(_createTokenRequest.AuthenticationType, identity.AuthenticationType);
            Assert.Equal(member.MemberId.ToString(), identity.FindFirst(IdentityClaimTypes.MemberId)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.MemberId)?.ValueType);
            Assert.Equal(member.DisplayName, identity.FindFirst(IdentityClaimTypes.DisplayName)?.Value);
            Assert.Equal(member.FirstName, identity.FindFirst(IdentityClaimTypes.FirstName)?.Value);
            Assert.Equal(member.LastName, identity.FindFirst(IdentityClaimTypes.LastName)?.Value);
            Assert.Equal(member.EmailAddress, identity.FindFirst(IdentityClaimTypes.EmailAddress)?.Value);
            Assert.Equal(member.PhoneNumber, identity.FindFirst(IdentityClaimTypes.PhoneNumber)?.Value);
            Assert.Equal(member.MemberSince.ToUnixTimeSeconds().ToString(), identity.FindFirst(IdentityClaimTypes.MemberSince)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.MemberSince)?.ValueType);
            Assert.Equal(member.RenewalDue.ToUnixTimeSeconds().ToString(), identity.FindFirst(IdentityClaimTypes.RenewalDue)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.RenewalDue)?.ValueType);
            Assert.Equal(member.BoardMemberType.ToString(), identity.FindFirst(IdentityClaimTypes.BoardMemberType)?.Value);
            Assert.Equal(_createTokenRequest.AuthenticationType, identity.FindFirst(IdentityClaimTypes.AuthenticationMethod)?.Value);
            Assert.Equal(_now.ToUnixTimeSeconds().ToString(), identity.FindFirst(IdentityClaimTypes.AuthenticationTime)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.AuthenticationTime)?.ValueType);
            Assert.Equal(TestIdentityProvider, identity.FindFirst(IdentityClaimTypes.IdentityProvider)?.Value);
            Assert.Equal(_createTokenRequest.Client.Id, identity.FindFirst(IdentityClaimTypes.ClientId)?.Value);
            Assert.Equal(_createTokenRequest.Nonce, identity.FindFirst(IdentityClaimTypes.Nonce)?.Value);

            Assert.True(identity.HasClaim(IdentityClaimTypes.Role, "test_role1"));
            Assert.True(identity.HasClaim(IdentityClaimTypes.Role, "test_role2"));
            Assert.True(identity.HasClaim(IdentityClaimTypes.Role, "test_role3"));

            Assert.True(identity.HasClaim(IdentityClaimTypes.ChampionArea, "test_area1"));
            Assert.True(identity.HasClaim(IdentityClaimTypes.ChampionArea, "test_area2"));
            Assert.True(identity.HasClaim(IdentityClaimTypes.ChampionArea, "test_area3"));
        }

        [Fact]
        public async Task<CreateTokenResult> CreateTokenAsync()
        {
            var createTokenResult = await _tokenService.CreateTokenAsync(_createTokenRequest).ConfigureAwait(false);
            Assert.NotNull(createTokenResult);
            Assert.True(createTokenResult.Success);

            AssertIdentity(createTokenResult.Subject);

            Assert.NotNull(createTokenResult.AccessToken);
            Assert.Null(createTokenResult.RefreshToken); // not implemented yet

            Assert.Equal(_now, createTokenResult.CreatedWhen);
            Assert.Equal(_now + _tokenOptions.AccessTokenLifetime, createTokenResult.AccessTokenExpiresWhen);

            return createTokenResult;
        }

        [Fact]
        public async Task ValidateTokenAsync_GivenValidToken()
        {
            var createTokenResult = await CreateTokenAsync().ConfigureAwait(false);

            var tokenValidationRequest = new TokenValidationRequest
            {
                Token = createTokenResult.AccessToken,
                AuthenticationType = _createTokenRequest.AuthenticationType,
                ValidAudiences = new[] { _createTokenRequest.Client.Id },
            };

            var tokenValidationResult = await _tokenService.ValidateTokenAsync(tokenValidationRequest).ConfigureAwait(false);
            Assert.NotNull(tokenValidationResult);
            Assert.True(tokenValidationResult.Success);

            var identity = Assert.IsType<ClaimsIdentity>(tokenValidationResult.Principal.Identity);
            AssertIdentity(identity);
        }

        [Fact]
        public async Task ValidateTokenAsync_GivenGarbageToken()
        {
            var tokenValidationRequest = new TokenValidationRequest
            {
                Token = "blah blah",
                AuthenticationType = _createTokenRequest.AuthenticationType,
                ValidAudiences = new[] { _createTokenRequest.Client.Id },
            };

            var tokenValidationResult = await _tokenService.ValidateTokenAsync(tokenValidationRequest).ConfigureAwait(false);
            Assert.NotNull(tokenValidationResult);
            Assert.False(tokenValidationResult.Success);

            Assert.Null(tokenValidationResult.Principal);
            Assert.NotNull(tokenValidationResult.Exception);
            Assert.Equal(ErrorCodes.InvalidGrant, tokenValidationResult.Error);
            Assert.StartsWith("IDX12741", tokenValidationResult.ErrorDescription);
        }

        [Fact]
        public async Task ValidateTokenAsync_GivenExpiredToken()
        {
            _tokenOptions.AccessTokenLifetime = TimeSpan.FromSeconds(1);
            _tokenOptions.ClockSkew = TimeSpan.Zero;

            var createTokenResult = await CreateTokenAsync().ConfigureAwait(false);

            await Task.Delay(_tokenOptions.AccessTokenLifetime).ConfigureAwait(false);

            var tokenValidationRequest = new TokenValidationRequest
            {
                Token = createTokenResult.AccessToken,
                AuthenticationType = _createTokenRequest.AuthenticationType,
                ValidAudiences = new[] { _createTokenRequest.Client.Id },
            };

            var tokenValidationResult = await _tokenService.ValidateTokenAsync(tokenValidationRequest).ConfigureAwait(false);
            Assert.NotNull(tokenValidationResult);
            Assert.False(tokenValidationResult.Success);

            Assert.Null(tokenValidationResult.Principal);
            Assert.NotNull(tokenValidationResult.Exception);
            Assert.Equal(ErrorCodes.ExpiredToken, tokenValidationResult.Error);
            Assert.Equal("Lifetime validation failed.", tokenValidationResult.ErrorDescription);
        }

        [Fact]
        public async Task ValidateTokenAsync_GivenInvalidAudience()
        {
            var createTokenResult = await CreateTokenAsync().ConfigureAwait(false);

            var tokenValidationRequest = new TokenValidationRequest
            {
                Token = createTokenResult.AccessToken,
                AuthenticationType = _createTokenRequest.AuthenticationType,
                ValidAudiences = new[] { Guid.NewGuid().ToString("N") },
            };

            var tokenValidationResult = await _tokenService.ValidateTokenAsync(tokenValidationRequest).ConfigureAwait(false);
            Assert.NotNull(tokenValidationResult);
            Assert.False(tokenValidationResult.Success);

            Assert.Null(tokenValidationResult.Principal);
            Assert.NotNull(tokenValidationResult.Exception);
            Assert.Equal(ErrorCodes.InvalidGrant, tokenValidationResult.Error);
            Assert.StartsWith("IDX10214: Audience validation failed.", tokenValidationResult.ErrorDescription);
        }

    }
}