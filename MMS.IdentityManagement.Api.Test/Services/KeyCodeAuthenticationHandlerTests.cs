using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.Services;
using MMS.IdentityManagement.Claims;
using MMS.IdentityManagement.Requests;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public class KeyCodeAuthenticationHandlerTests
    {
        private readonly Mock<IClientValidator> _mockClientValidator = new Mock<IClientValidator>(MockBehavior.Strict);
        private readonly Mock<IKeyCodeValidator> _mockKeyCodeValidator = new Mock<IKeyCodeValidator>(MockBehavior.Strict);
        private readonly Mock<ITokenService> _mockTokenService = new Mock<ITokenService>(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _mockSystemClock = new Mock<ISystemClock>(MockBehavior.Strict);

        [Fact]
        public async Task Authenticate_GivenInvalidClient_ThenUnauthorizedClient()
        {
            var cancellationToken = CancellationToken.None;

            var request = new KeyCodeAuthenticationRequest
            {
                ClientId = Guid.NewGuid().ToString("N"),
                ClientSecret = Guid.NewGuid().ToString("N"),
                KeyCode = Guid.NewGuid().ToString("N"),
            };

            _mockClientValidator
                .Setup(_ => _.ValidateClientAsync(It.IsAny<ClientValidationRequest>(), cancellationToken))
                .Callback((ClientValidationRequest arg0, CancellationToken arg1) =>
                {
                    Assert.Equal("keycode", arg0.AuthenticationType);
                    Assert.Equal(request.ClientId, arg0.ClientId);
                    Assert.Equal(request.ClientSecret, arg0.ClientSecret);
                })
                .ReturnsAsync(new ClientValidationResult
                {
                    Error = "unauthorized_client",
                    ErrorDescription = "from unit tests",
                });

            var handler = new KeyCodeAuthenticationHandler(
                _mockClientValidator.Object,
                _mockKeyCodeValidator.Object,
                _mockTokenService.Object,
                _mockSystemClock.Object);

            var result = await handler.AuthenticateAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("unauthorized_client", result.Error);
            Assert.Equal("from unit tests", result.ErrorDescription);
        }

        [Fact]
        public async Task Authenticate_GivenInvalidKeyCode_ThenInvalidGrant()
        {
            var now = DateTimeOffset.Now;
            var cancellationToken = CancellationToken.None;

            var secret = new Secret
            {
                Id = Guid.NewGuid().ToString("N"),
                CipherType = "test",
                CipherText = Guid.NewGuid().ToString("N"),
                CreatedWhen = now,
                UpdatedWhen = now,
            };
            var client = new Client
            {
                Id = Guid.NewGuid().ToString("N"),
                Disabled = false,
                RequireSecret = true,
                Secrets = new[] { secret },
            };
            var request = new KeyCodeAuthenticationRequest
            {
                ClientId = Guid.NewGuid().ToString("N"),
                ClientSecret = Guid.NewGuid().ToString("N"),
                KeyCode = Guid.NewGuid().ToString("N"),
            };

            _mockClientValidator
                .Setup(_ => _.ValidateClientAsync(It.IsAny<ClientValidationRequest>(), cancellationToken))
                .Callback((ClientValidationRequest arg0, CancellationToken arg1) =>
                {
                    Assert.Equal("keycode", arg0.AuthenticationType);
                    Assert.Equal(request.ClientId, arg0.ClientId);
                    Assert.Equal(request.ClientSecret, arg0.ClientSecret);
                })
                .ReturnsAsync(new ClientValidationResult
                {
                    Client = client,
                    Secret = secret,
                });

            _mockKeyCodeValidator
                .Setup(_ => _.ValidateKeyCodeAsync(It.IsAny<KeyCodeValidationRequest>(), cancellationToken))
                .Callback((KeyCodeValidationRequest arg0, CancellationToken arg1) =>
                {
                    Assert.Equal(request.KeyCode, arg0.KeyCode);
                })
                .ReturnsAsync(new KeyCodeValidationResult
                {
                    Error = "invalid_grant",
                    ErrorDescription = "from unit tests",
                });

            var handler = new KeyCodeAuthenticationHandler(
                _mockClientValidator.Object,
                _mockKeyCodeValidator.Object,
                _mockTokenService.Object,
                _mockSystemClock.Object);

            var result = await handler.AuthenticateAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("invalid_grant", result.Error);
            Assert.Equal("from unit tests", result.ErrorDescription);
        }

        [Fact]
        public async Task Authenticate_GivenValidRequest_ThenAuthorized()
        {
            var now = DateTimeOffset.Now;
            var cancellationToken = CancellationToken.None;

            var secret = new Secret
            {
                Id = Guid.NewGuid().ToString("N"),
                CipherType = "test",
                CipherText = Guid.NewGuid().ToString("N"),
                CreatedWhen = now,
                UpdatedWhen = now,
            };
            var client = new Client
            {
                Id = Guid.NewGuid().ToString("N"),
                Disabled = false,
                RequireSecret = true,
                Secrets = new[] { secret },
            };
            var member = new Member
            {
                MemberId = 1,
                DisplayName = "John Doe",
                FirstName = "John",
                LastName = "Doe",
                MembershipCreatedWhen = now.AddMonths(-1),
                MembershipExpiresWhen = now.AddMonths(1),
            };
            var identity = new ClaimsIdentity("keycode");
            identity.AddClaim(new Claim(IdentityClaimTypes.MemberId, member.MemberId.ToString()));
            identity.AddClaim(new Claim(IdentityClaimTypes.DisplayName, member.DisplayName));
            identity.AddClaim(new Claim(IdentityClaimTypes.FirstName, member.FirstName));
            identity.AddClaim(new Claim(IdentityClaimTypes.LastName, member.LastName));

            var request = new KeyCodeAuthenticationRequest
            {
                ClientId = client.Id,
                ClientSecret = Guid.NewGuid().ToString("N"),
                KeyCode = Guid.NewGuid().ToString("N"),
            };

            _mockClientValidator
                .Setup(_ => _.ValidateClientAsync(It.IsAny<ClientValidationRequest>(), cancellationToken))
                .Callback((ClientValidationRequest arg0, CancellationToken arg1) =>
                {
                    Assert.Equal("keycode", arg0.AuthenticationType);
                    Assert.Equal(request.ClientId, arg0.ClientId);
                    Assert.Equal(request.ClientSecret, arg0.ClientSecret);
                })
                .ReturnsAsync(new ClientValidationResult
                {
                    Client = client,
                    Secret = secret,
                });

            _mockKeyCodeValidator
                .Setup(_ => _.ValidateKeyCodeAsync(It.IsAny<KeyCodeValidationRequest>(), cancellationToken))
                .Callback((KeyCodeValidationRequest arg0, CancellationToken arg1) =>
                {
                    Assert.Equal(request.KeyCode, arg0.KeyCode);
                })
                .ReturnsAsync(new KeyCodeValidationResult
                {
                    Member = member,
                });

            _mockTokenService
                .Setup(_ => _.CreateTokenAsync(It.IsAny<CreateTokenRequest>(), cancellationToken))
                .ReturnsAsync(new CreateTokenResult
                {
                    AccessToken = "test_access_token",
                    AccessTokenExpiresWhen = now.AddDays(1),
                    CreatedWhen = now,
                    RefreshToken = "test_refresh_token",
                    Identity = identity,
                });

            _mockSystemClock
                .Setup(_ => _.UtcNow)
                .Returns(now);

            var handler = new KeyCodeAuthenticationHandler(
                _mockClientValidator.Object,
                _mockKeyCodeValidator.Object,
                _mockTokenService.Object,
                _mockSystemClock.Object);

            var result = await handler.AuthenticateAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Bearer", result.TokenType);
            Assert.Equal("test_access_token", result.IdentityToken);
            Assert.Equal("test_access_token", result.AccessToken);
            Assert.Equal(now.AddDays(1), result.AccessTokenExpiresWhen);
            Assert.Equal("test_refresh_token", result.RefreshToken);
        }

    }
}