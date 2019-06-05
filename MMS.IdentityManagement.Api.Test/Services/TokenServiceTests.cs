using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.Options;
using MMS.IdentityManagement.Api.Services;
using MMS.IdentityManagement.Claims;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public class TokenServiceTests
    {
        [Fact]
        public async Task CreateTokenAsync()
        {
            var now = DateTimeOffset.UtcNow;
            var mockSystemClock = new Mock<ISystemClock>(MockBehavior.Strict);
            mockSystemClock
                .Setup(_ => _.UtcNow)
                .Returns(now)
                .Verifiable();

            var tokenOptions = new TokenOptions
            {
            };
            var tokenOptionsAccessor = Microsoft.Extensions.Options.Options.Create(tokenOptions);

            var tokenService = new TokenService(tokenOptionsAccessor, mockSystemClock.Object);

            var cancellationToken = CancellationToken.None;
            var request = new CreateTokenRequest
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
                            CreatedWhen = now,
                            UpdatedWhen = now,
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

                    MemberSince = now.AddMonths(-1),
                    RenewalDue = now.AddMonths(1),

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
                AuthenticationTime = now,
                Nonce = Guid.NewGuid().ToString("N"),
            };

            var result = await tokenService.CreateTokenAsync(request, cancellationToken).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.True(result.Success);

            var member = request.Member;
            Assert.NotNull(result.Subject);
            Assert.Equal(request.AuthenticationType, result.Subject.AuthenticationType);
            Assert.Equal(member.MemberId.ToString(), result.Subject.FindFirst(IdentityClaimTypes.MemberId)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, result.Subject.FindFirst(IdentityClaimTypes.MemberId)?.ValueType);
            Assert.Equal(member.DisplayName, result.Subject.FindFirst(IdentityClaimTypes.DisplayName)?.Value);
            Assert.Equal(member.FirstName, result.Subject.FindFirst(IdentityClaimTypes.FirstName)?.Value);
            Assert.Equal(member.LastName, result.Subject.FindFirst(IdentityClaimTypes.LastName)?.Value);
            Assert.Equal(member.EmailAddress, result.Subject.FindFirst(IdentityClaimTypes.EmailAddress)?.Value);
            Assert.Equal(ClaimValueTypes.Email, result.Subject.FindFirst(IdentityClaimTypes.EmailAddress)?.ValueType);
            Assert.Equal(member.PhoneNumber, result.Subject.FindFirst(IdentityClaimTypes.PhoneNumber)?.Value);
            Assert.Equal(member.MemberSince.ToUnixTimeSeconds().ToString(), result.Subject.FindFirst(IdentityClaimTypes.MemberSince)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, result.Subject.FindFirst(IdentityClaimTypes.MemberSince)?.ValueType);
            Assert.Equal(member.RenewalDue.ToUnixTimeSeconds().ToString(), result.Subject.FindFirst(IdentityClaimTypes.RenewalDue)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, result.Subject.FindFirst(IdentityClaimTypes.RenewalDue)?.ValueType);
            Assert.Equal(member.BoardMemberType.ToString(), result.Subject.FindFirst(IdentityClaimTypes.BoardMemberType)?.Value);
            Assert.Equal(request.AuthenticationType, result.Subject.FindFirst(IdentityClaimTypes.AuthenticationMethod)?.Value);
            Assert.Equal(now.ToUnixTimeSeconds().ToString(), result.Subject.FindFirst(IdentityClaimTypes.AuthenticationTime)?.Value);
            Assert.Equal(ClaimValueTypes.Integer, result.Subject.FindFirst(IdentityClaimTypes.AuthenticationTime)?.ValueType);
            Assert.Equal(tokenOptions.IdentityProvider, result.Subject.FindFirst(IdentityClaimTypes.IdentityProvider)?.Value);
            Assert.Equal(request.Client.Id, result.Subject.FindFirst(IdentityClaimTypes.ClientId)?.Value);
            Assert.Equal(request.Nonce, result.Subject.FindFirst(IdentityClaimTypes.Nonce)?.Value);

            Assert.True(result.Subject.HasClaim(IdentityClaimTypes.Role, "test_role1"));
            Assert.True(result.Subject.HasClaim(IdentityClaimTypes.Role, "test_role2"));
            Assert.True(result.Subject.HasClaim(IdentityClaimTypes.Role, "test_role3"));

            Assert.True(result.Subject.HasClaim(IdentityClaimTypes.ChampionArea, "test_area1"));
            Assert.True(result.Subject.HasClaim(IdentityClaimTypes.ChampionArea, "test_area2"));
            Assert.True(result.Subject.HasClaim(IdentityClaimTypes.ChampionArea, "test_area3"));

            Assert.NotNull(result.AccessToken);

            Assert.Equal(now, result.CreatedWhen);
            Assert.Equal(now + tokenOptions.AccessTokenLifetime, result.AccessTokenExpiresWhen);
        }

    }
}