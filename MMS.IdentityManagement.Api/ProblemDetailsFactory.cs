using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace MMS.IdentityManagement.Api
{
    public interface IProblemDetailsFactory
    {
        ProblemDetails FromServerError(HttpContext httpContext, Exception exception);

        ProblemDetails FromClientError(ActionContext actionContext, IClientErrorActionResult clientError);
    }

    public class ExceptionToStatusCodeMapping
    {
        public Type ExceptionType { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class ProblemDetailsFactory : IProblemDetailsFactory
    {
        // NOTE: order matters
        private static readonly IReadOnlyList<ExceptionToStatusCodeMapping> Mappings = new[]
        {
            new ExceptionToStatusCodeMapping
            {
                ExceptionType = typeof(UnauthorizedAccessException),
                StatusCode = StatusCodes.Status401Unauthorized,
            },
            new ExceptionToStatusCodeMapping
            {
                ExceptionType = typeof(OperationCanceledException),
                StatusCode = 499,
                StatusMessage = "Client Closed Request",
            },
            new ExceptionToStatusCodeMapping
            {
                ExceptionType = typeof(NotImplementedException),
                StatusCode = StatusCodes.Status501NotImplemented,
            },
            // the following mapping must be last as a catch-all
            new ExceptionToStatusCodeMapping
            {
                ExceptionType = typeof(Exception),
                StatusCode = StatusCodes.Status500InternalServerError,
            },
        };

        protected void AddCommonExtensions(ProblemDetails problemDetails, HttpContext httpContext)
        {
            var machineName = Environment.MachineName;
            var timestamp = DateTimeOffset.Now;
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            problemDetails.Extensions["machineName"] = machineName;
            problemDetails.Extensions["timestamp"] = timestamp;
            problemDetails.Extensions["traceId"] = traceId;
        }

        public virtual ProblemDetails FromServerError(HttpContext httpContext, Exception exception)
        {
            var baseException = exception.GetBaseException();
            var mapping = Mappings.First(_ => _.ExceptionType.IsInstanceOfType(baseException));

            var statusCode = mapping.StatusCode;
            var statusMessage = mapping.StatusMessage ?? ReasonPhrases.GetReasonPhrase(statusCode);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusMessage,
                Extensions =
                {
                    ["exception"] = baseException,
                },
            };

            AddCommonExtensions(problemDetails, httpContext);

            return problemDetails;
        }

        public virtual ProblemDetails FromClientError(ActionContext actionContext, IClientErrorActionResult clientError)
        {
            var statusCode = clientError?.StatusCode ?? StatusCodes.Status400BadRequest;

            ProblemDetails problemDetails;
            if (!actionContext.ModelState.IsValid)
            {
                problemDetails = new ValidationProblemDetails(actionContext.ModelState)
                {
                    Status = statusCode,
                    // Title is already set by the ctor
                };
            }
            else
            {
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = ReasonPhrases.GetReasonPhrase(statusCode),
                };
            }

            AddCommonExtensions(problemDetails, actionContext.HttpContext);

            return problemDetails;
        }

    }
}