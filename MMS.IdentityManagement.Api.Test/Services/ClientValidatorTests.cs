using System.Collections.Generic;
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
        public static IEnumerable<object[]> TestData => new[]
        {
            new object[]
            {
                new ClientValidationRequest
                {
                    // nothing
                },
                "invalid_request",
                "Missing client_id",
            },
            new object[]
            {
                new ClientValidationRequest
                {
                    ClientId = string.Empty,
                },
                "invalid_request",
                "Missing client_id",
            },
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task ValidateClientAsync_GivenError_ThenFailedResult(ClientValidationRequest request, string expectedError, string expectedErrorDescription)
        {
            var mockRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var clientValidator = new ClientValidator(mockRepository.Object, mockSecretProtectorSelector.Object);
            var result = await clientValidator.ValidateClientAsync(request).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal(expectedError, result.Error);
            Assert.Equal(expectedErrorDescription, result.ErrorDescription);
        }

        [Fact]
        public async Task ValidateClientAsync_GivenMissingClientId_ThenInvalidRequest()
        {
            var mockRepository = new Mock<IClientRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);
            //var mockSecretProtector = new Mock<ISecretProtector>(MockBehavior.Strict);

            //const string cipherType = "";

            //mockSecretProtectorSelector
            //    .Setup(_ => _.Select(cipherType))
            //    .Returns(mockSecretProtector.Object)
            //    .Verifiable();

            //mockSecretProtector
            //    .Setup(_ => _.CipherType)
            //    .Returns(cipherType)
            //    .Verifiable();

            var clientValidator = new ClientValidator(mockRepository.Object, mockSecretProtectorSelector.Object);

            var request = new ClientValidationRequest();
            var result = await clientValidator.ValidateClientAsync(request).ConfigureAwait(false);

            Assert.False(result.Success);
            Assert.Equal("invalid_request", result.Error);
            Assert.Equal("Missing client_id", result.ErrorDescription);
        }

    }
}