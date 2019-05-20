using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IProblemDetailsFactory
    {
        ProblemDetails FromServerError(HttpContext httpContext, Exception exception);

        ProblemDetails FromClientError(ActionContext actionContext, IClientErrorActionResult clientError);

        ProblemDetails FromValidationResult(ActionContext actionContext, ValidationResult validationResult);
    }

    public class ProblemDetailsFactory : IProblemDetailsFactory
    {
        // NOTE: order matters
        private static readonly IReadOnlyList<(Type ExceptionType, int StatusCode, string StatusMessage)> Mappings
            = new (Type ExceptionType, int StatusCode, string StatusMessage)[]
            {
                (typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized, null),
                (typeof(OperationCanceledException), 499, "Client Closed Request"),
                (typeof(NotImplementedException), StatusCodes.Status501NotImplemented, null),
                // the following mapping must be last as a catch-all
                (typeof(Exception), StatusCodes.Status500InternalServerError, null),
            };

        protected void AddCommonDetails(ProblemDetails problemDetails, HttpContext httpContext)
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

            AddCommonDetails(problemDetails, httpContext);

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

            AddCommonDetails(problemDetails, actionContext.HttpContext);

            return problemDetails;
        }

        public virtual ProblemDetails FromValidationResult(ActionContext actionContext, ValidationResult validationResult)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = validationResult.Error,
                Detail = validationResult.ErrorDescription,
            };

            AddCommonDetails(problemDetails, actionContext.HttpContext);

            return problemDetails;
        }

    }
}