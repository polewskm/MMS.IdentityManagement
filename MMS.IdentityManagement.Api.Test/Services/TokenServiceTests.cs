using System;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Api.Extensions;
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
                    ClientId = "test_client",
                    RequireSecret = true,
                    Disabled = false,
                    Secrets = new[]
                    {
                        new Secret
                        {
                            SecretId = "test_secret",
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

                    MembershipCreatedWhen = _now.AddMonths(-1),
                    MembershipExpiresWhen = _now.AddMonths(1),

                    Roles = new[]
                    {
                        "test_role1",
                        "test_role2",
                        "test_role3",
                    },
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

            Assert.Equal(member.MemberId, identity.GetMemberId());
            Assert.Equal(member.MemberId.ToString(), identity.FindFirst(IdentityClaimTypes.MemberId)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.MemberId)?.ValueType);

            Assert.Equal(member.DisplayName, identity.GetDisplayName());
            Assert.Equal(member.DisplayName, identity.FindFirst(IdentityClaimTypes.DisplayName)?.Value);

            Assert.Equal(member.FirstName, identity.GetFirstName());
            Assert.Equal(member.FirstName, identity.FindFirst(IdentityClaimTypes.FirstName)?.Value);

            Assert.Equal(member.LastName, identity.GetLastName());
            Assert.Equal(member.LastName, identity.FindFirst(IdentityClaimTypes.LastName)?.Value);

            Assert.Equal(member.EmailAddress, identity.GetEmailAddress());
            Assert.Equal(member.EmailAddress, identity.FindFirst(IdentityClaimTypes.EmailAddress)?.Value);

            Assert.Equal(member.PhoneNumber, identity.GetPhoneNumber());
            Assert.Equal(member.PhoneNumber, identity.FindFirst(IdentityClaimTypes.PhoneNumber)?.Value);

            Assert.Equal(member.MembershipCreatedWhen.TruncateMilliseconds(), identity.GetMembershipCreatedWhen());
            Assert.Equal(member.MembershipCreatedWhen.TruncateMilliseconds().ToUnixTimeSeconds().ToString(), identity.FindFirst(IdentityClaimTypes.MembershipCreatedWhen)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.MembershipCreatedWhen)?.ValueType);

            Assert.Equal(member.MembershipExpiresWhen.TruncateMilliseconds(), identity.GetMembershipExpiresWhen());
            Assert.Equal(member.MembershipExpiresWhen.TruncateMilliseconds().ToUnixTimeSeconds().ToString(), identity.FindFirst(IdentityClaimTypes.MembershipExpiresWhen)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.MembershipExpiresWhen)?.ValueType);

            Assert.Equal(_createTokenRequest.AuthenticationType, identity.FindFirst(IdentityClaimTypes.AuthenticationMethod)?.Value);

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            // false positive
            Assert.Equal(_now.ToUnixTimeSeconds().ToString(), identity.FindFirst(IdentityClaimTypes.AuthenticationTime)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, identity.FindFirst(IdentityClaimTypes.AuthenticationTime)?.ValueType);

            Assert.True(identity.HasClaim(IdentityClaimTypes.Role, "test_role1"));
            Assert.True(identity.HasClaim(IdentityClaimTypes.Role, "test_role2"));
            Assert.True(identity.HasClaim(IdentityClaimTypes.Role, "test_role3"));
        }

        [Fact]
        public async Task<CreateTokenResult> CreateTokenAsync()
        {
            var createTokenResult = await _tokenService.CreateTokenAsync(_createTokenRequest).ConfigureAwait(false);
            Assert.NotNull(createTokenResult);
            Assert.True(createTokenResult.Success);

            AssertIdentity(createTokenResult.Identity);

            Assert.NotNull(createTokenResult.AccessToken);
            Assert.Null(createTokenResult.RefreshToken); // TODO: not implemented yet

            Assert.Equal(_now.TruncateMilliseconds(), createTokenResult.CreatedWhen);
            Assert.Equal((_now + _tokenOptions.AccessTokenLifetime).TruncateMilliseconds(), createTokenResult.AccessTokenExpiresWhen);
            Assert.Equal((_now + _tokenOptions.RefreshTokenLifetime).TruncateMilliseconds(), createTokenResult.RefreshTokenExpiresWhen);

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
                ValidAudiences = new[] { _createTokenRequest.Client.ClientId },
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
                ValidAudiences = new[] { _createTokenRequest.Client.ClientId },
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
                ValidAudiences = new[] { _createTokenRequest.Client.ClientId },
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