using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MMS.IdentityManagement.Api.Services;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public class ClientErrorFactoryTests
    {
        private static void AssertGetClientError<TActionResult>(int statusCode)
            where TActionResult : ObjectResult
        {
            var actionContext = new ActionContext();
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
            };

            var mockProblemDetailsFactory = new Mock<IProblemDetailsFactory>(MockBehavior.Strict);
            var mockClientErrorActionResult = new Mock<IClientErrorActionResult>(MockBehavior.Strict);

            mockProblemDetailsFactory
                .Setup(_ => _.FromClientError(actionContext, mockClientErrorActionResult.Object))
                .Returns(problemDetails)
                .Verifiable();

            var clientErrorFactory = new ClientErrorFactory(mockProblemDetailsFactory.Object);
            var actionResult = clientErrorFactory.GetClientError(actionContext, mockClientErrorActionResult.Object);

            var objectResult = Assert.IsType<TActionResult>(actionResult);
            Assert.Equal(statusCode, objectResult.StatusCode);
            Assert.Same(problemDetails, objectResult.Value);

            Assert.Contains("application/problem+json", objectResult.ContentTypes);
            Assert.Contains("application/problem+xml", objectResult.ContentTypes);

            mockProblemDetailsFactory.Verify();
        }

        [Fact]
        public void GetClientError_Given200_ThenOkObjectResult()
        {
            AssertGetClientError<OkObjectResult>(StatusCodes.Status200OK);
        }

        [Fact]
        public void GetClientError_Given204_ThenObjectResult()
        {
            AssertGetClientError<ObjectResult>(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void GetClientError_Given400_ThenBadRequestObjectResult()
        {
            AssertGetClientError<BadRequestObjectResult>(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public void GetClientError_Given401_ThenUnauthorizedObjectResult()
        {
            AssertGetClientError<UnauthorizedObjectResult>(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public void GetClientError_Given404_ThenNotFoundObjectResult()
        {
            AssertGetClientError<NotFoundObjectResult>(StatusCodes.Status404NotFound);
        }

        [Fact]
        public void GetClientError_Given409_ThenConflictObjectResult()
        {
            AssertGetClientError<ConflictObjectResult>(StatusCodes.Status409Conflict);
        }

        [Fact]
        // ReSharper disable once IdentifierTypo
        public void GetClientError_Given422_ThenUnprocessableEntityObjectResult()
        {
            AssertGetClientError<UnprocessableEntityObjectResult>(StatusCodes.Status422UnprocessableEntity);
        }

    }
}