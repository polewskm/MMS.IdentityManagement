using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Data;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.SecretProtectors;
using MMS.IdentityManagement.Api.Services;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public class ClientValidatorTests
    {
        [Fact]
        public async Task ValidateClientAsync_GivenMissingClientId_ThenInvalidRequest()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var request = new ClientValidationRequest();
            var result = await clientValidator.ValidateClientAsync(request).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("invalid_request", result.Error);
            Assert.Equal("Missing client_id", result.ErrorDescription);
        }

        [Fact]
        public async Task ValidateClientAsync_GivenEmptyClientId_ThenInvalidRequest()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var request = new ClientValidationRequest
            {
                ClientId = string.Empty
            };
            var result = await clientValidator.ValidateClientAsync(request).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("invalid_request", result.Error);
            Assert.Equal("Missing client_id", result.ErrorDescription);
        }

        [Fact]
        public async Task ValidateClientAsync_GivenUnknownClientId_ThenInvalidClient()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var clientId = Guid.NewGuid().ToString("N");
            var cancellationToken = CancellationToken.None;

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync((Client)null)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("invalid_client", result.Error);
            Assert.Equal("Unknown client_id", result.ErrorDescription);
            Assert.Null(result.Client);
            Assert.Null(result.Secret);

            mockClientRepository.Verify();
        }

        [Fact]
        public async Task ValidateClientAsync_GivenValidClientId_ThenValidClient()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var clientId = Guid.NewGuid().ToString("N");
            var cancellationToken = CancellationToken.None;
            var client = new Client
            {
                Id = clientId,
                Disabled = false,
                RequireSecret = false,
            };

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync(client)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.Null(result.ErrorDescription);
            Assert.Same(client, result.Client);

            mockClientRepository.Verify();
        }

        [Fact]
        public async Task ValidateClientAsync_GivenDisabledClientId_ThenUnauthorizedClient()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var clientId = Guid.NewGuid().ToString("N");
            var cancellationToken = CancellationToken.None;
            var client = new Client
            {
                Id = clientId,
                Disabled = true,
                RequireSecret = false,
            };

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync(client)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("unauthorized_client", result.Error);
            Assert.Equal("Disabled client", result.ErrorDescription);
            Assert.Null(result.Client);

            mockClientRepository.Verify();
        }

        [Fact]
        public async Task ValidateClientAsync_GivenNullSecret_ThenInvalidRequest()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var clientId = Guid.NewGuid().ToString("N");
            var cancellationToken = CancellationToken.None;
            var client = new Client
            {
                Id = clientId,
                Disabled = false,
                RequireSecret = true,
            };

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync(client)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
                ClientSecret = null,
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("invalid_request", result.Error);
            Assert.Equal("Missing client_secret", result.ErrorDescription);
            Assert.Null(result.Client);

            mockClientRepository.Verify();
        }

        [Fact]
        public async Task ValidateClientAsync_GivenEmptySecret_ThenInvalidRequest()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var clientId = Guid.NewGuid().ToString("N");
            var cancellationToken = CancellationToken.None;
            var client = new Client
            {
                Id = clientId,
                Disabled = false,
                RequireSecret = true,
            };

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync(client)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
                ClientSecret = null,
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("invalid_request", result.Error);
            Assert.Equal("Missing client_secret", result.ErrorDescription);
            Assert.Null(result.Client);

            mockClientRepository.Verify();
        }

        [Fact]
        public async Task ValidateClientAsync_GivenInvalidSecret_ThenUnauthorizedClient()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var clientId = Guid.NewGuid().ToString("N");
            var cancellationToken = CancellationToken.None;
            var client = new Client
            {
                Id = clientId,
                Disabled = false,
                RequireSecret = true,
            };

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync(client)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
                ClientSecret = Guid.NewGuid().ToString("N"),
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("unauthorized_client", result.Error);
            Assert.Equal("Invalid client_secret", result.ErrorDescription);
            Assert.Null(result.Client);

            mockClientRepository.Verify();
        }

        [Fact]
        public async Task ValidateClientAsync_GivenValidSecret_ThenAuthorizedClient()
        {
            var mockClientRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);
            var mockSecretProtector = new Mock<ISecretProtector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockClientRepository.Object, mockSecretProtectorSelector.Object);

            var now = DateTimeOffset.Now;
            var cancellationToken = CancellationToken.None;

            var clientId = Guid.NewGuid().ToString("N");

            const string cipherType = "test_cipher";
            var clientSecret = Guid.NewGuid().ToString("N");

            var secret = new Secret
            {
                Id = "match",
                CipherType = cipherType,
                CipherText = Guid.NewGuid().ToString("N"),
                CreatedWhen = now,
                UpdatedWhen = now,
            };

            var client = new Client
            {
                Id = clientId,
                Disabled = false,
                RequireSecret = true,
                Secrets = new[]
                {
                    new Secret
                    {
                        Id = "other",
                        CipherType = cipherType,
                        CipherText = Guid.NewGuid().ToString("N"),
                        CreatedWhen = now,
                        UpdatedWhen = now,
                    },
                    secret,
                }
            };

            mockClientRepository
                .Setup(_ => _.GetClientByIdAsync(clientId, cancellationToken))
                .ReturnsAsync(client)
                .Verifiable();

            mockSecretProtectorSelector
                .Setup(_ => _.Select(cipherType))
                .Returns(mockSecretProtector.Object)
                .Verifiable();

            mockSecretProtector
                .Setup(_ => _.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            mockSecretProtector
                .Setup(_ => _.Verify(clientSecret, secret.CipherText))
                .Returns(true)
                .Verifiable();

            var request = new ClientValidationRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
            };
            var result = await clientValidator.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);

            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.Null(result.ErrorDescription);
            Assert.Same(client, result.Client);
            Assert.Same(secret, result.Secret);

            mockClientRepository.Verify();
            mockSecretProtectorSelector.Verify();
            mockSecretProtector.Verify();
        }

    }
}