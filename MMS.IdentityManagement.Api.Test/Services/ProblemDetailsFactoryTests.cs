using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MMS.IdentityManagement.Api.Services;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public class ProblemDetailsFactoryTests
    {
        private readonly string _traceId = Guid.NewGuid().ToString("N");

        private void AssertCommonDetails(IDictionary<string, object> extensions)
        {
            Assert.Contains("machineName", extensions);
            Assert.Equal(Environment.MachineName, extensions["machineName"]);

            Assert.Contains("traceId", extensions);
            Assert.Equal(_traceId, extensions["traceId"]);
        }

        [Fact]
        public void FromServerError_GivenUnauthorizedAccessException_Then401Unauthorized()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var exception = new UnauthorizedAccessException();

            var problemDetails = problemDetailsFactory.FromServerError(mockHttpContext.Object, exception);

            Assert.NotNull(problemDetails);
            Assert.Equal(401, problemDetails.Status);
            Assert.Equal("Unauthorized", problemDetails.Title);

            Assert.Contains("exception", problemDetails.Extensions);
            Assert.Same(exception, problemDetails.Extensions["exception"]);

            AssertCommonDetails(problemDetails.Extensions);

            mockHttpContext.Verify();
        }

        [Fact]
        public void FromServerError_GivenOperationCanceledException_Then499ClientClosedRequest()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var exception = new OperationCanceledException();

            var problemDetails = problemDetailsFactory.FromServerError(mockHttpContext.Object, exception);

            Assert.NotNull(problemDetails);
            Assert.Equal(499, problemDetails.Status);
            Assert.Equal("Client Closed Request", problemDetails.Title);

            Assert.Contains("exception", problemDetails.Extensions);
            Assert.Same(exception, problemDetails.Extensions["exception"]);

            AssertCommonDetails(problemDetails.Extensions);

            mockHttpContext.Verify();
        }

        [Fact]
        public void FromServerError_GivenTaskCanceledException_Then499ClientClosedRequest()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var exception = new TaskCanceledException();

            var problemDetails = problemDetailsFactory.FromServerError(mockHttpContext.Object, exception);

            Assert.NotNull(problemDetails);
            Assert.Equal(499, problemDetails.Status);
            Assert.Equal("Client Closed Request", problemDetails.Title);

            Assert.Contains("exception", problemDetails.Extensions);
            Assert.Same(exception, problemDetails.Extensions["exception"]);

            AssertCommonDetails(problemDetails.Extensions);

            mockHttpContext.Verify();
        }

        [Fact]
        public void FromServerError_GivenNotImplementedException_Then501NotImplemented()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var exception = new NotImplementedException();

            var problemDetails = problemDetailsFactory.FromServerError(mockHttpContext.Object, exception);

            Assert.NotNull(problemDetails);
            Assert.Equal(501, problemDetails.Status);
            Assert.Equal("Not Implemented", problemDetails.Title);

            Assert.Contains("exception", problemDetails.Extensions);
            Assert.Same(exception, problemDetails.Extensions["exception"]);

            AssertCommonDetails(problemDetails.Extensions);

            mockHttpContext.Verify();
        }

        [Fact]
        public void FromServerError_GivenException_Then500InternalServerError()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var exception = new Exception();

            var problemDetails = problemDetailsFactory.FromServerError(mockHttpContext.Object, exception);

            Assert.NotNull(problemDetails);
            Assert.Equal(500, problemDetails.Status);
            Assert.Equal("Internal Server Error", problemDetails.Title);

            Assert.Contains("exception", problemDetails.Extensions);
            Assert.Same(exception, problemDetails.Extensions["exception"]);

            AssertCommonDetails(problemDetails.Extensions);

            mockHttpContext.Verify();
        }

        [Fact]
        public void FromClientError_GivenInvalidModelState_ThenValidationProblems()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var actionContext = new ActionContext
            {
                HttpContext = mockHttpContext.Object,
            };

            actionContext.ModelState.AddModelError("prop1", "message1.1");
            actionContext.ModelState.AddModelError("prop1", "message1.2");

            var mockClientErrorActionResult = new Mock<IClientErrorActionResult>(MockBehavior.Strict);
            mockClientErrorActionResult
                .Setup(_ => _.StatusCode)
                .Returns((int?)null)
                .Verifiable();

            var problemDetails = problemDetailsFactory.FromClientError(actionContext, mockClientErrorActionResult.Object);

            Assert.NotNull(problemDetails);
            Assert.Equal(400, problemDetails.Status);
            Assert.Equal("One or more validation errors occurred.", problemDetails.Title);

            AssertCommonDetails(problemDetails.Extensions);

            var validationProblems = Assert.IsType<ValidationProblemDetails>(problemDetails);
            Assert.Equal("{\"prop1\":[\"message1.1\",\"message1.2\"]}", validationProblems.Errors.ToJson());

            mockHttpContext.Verify();
            mockClientErrorActionResult.Verify();
        }

        [Fact]
        public void FromClientError_GivenActionResult_ThenProblemDetails()
        {
            var problemDetailsFactory = new ProblemDetailsFactory();

            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockHttpContext
                .Setup(_ => _.TraceIdentifier)
                .Returns(_traceId)
                .Verifiable();

            var actionContext = new ActionContext
            {
                HttpContext = mockHttpContext.Object,
            };

            var mockClientErrorActionResult = new Mock<IClientErrorActionResult>(MockBehavior.Strict);
            mockClientErrorActionResult
                .Setup(_ => _.StatusCode)
                .Returns(404)
                .Verifiable();

            var problemDetails = problemDetailsFactory.FromClientError(actionContext, mockClientErrorActionResult.Object);

            Assert.NotNull(problemDetails);
            Assert.Equal(404, problemDetails.Status);
            Assert.Equal("Not Found", problemDetails.Title);

            AssertCommonDetails(problemDetails.Extensions);

            mockHttpContext.Verify();
            mockClientErrorActionResult.Verify();
        }

    }
}