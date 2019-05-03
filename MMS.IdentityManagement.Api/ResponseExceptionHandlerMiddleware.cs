﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Api
{
    public class ResponseExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate"/></param>
        public ResponseExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate"/></param>
        /// <param name="settings"><see cref="JsonSerializerSettings"/></param>
        public ResponseExceptionHandlerMiddleware(RequestDelegate next, JsonSerializerSettings settings)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _settings = settings; // nullable
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="httpContext"><see cref="HttpContext"/></param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException exception)
            {
                // unauthorized exceptions return the 403 HTTP status code
                if (!await HandleExceptionAsync(httpContext, exception, StatusCodes.Status403Forbidden, exception.Message).ConfigureAwait(false))
                    throw;
            }
            catch (OperationCanceledException exception)
            {
                // canceled operations return the 499 HTTP status code
                if (!await HandleExceptionAsync(httpContext, exception, 499, exception.Message).ConfigureAwait(false))
                    throw;
            }
            catch (NotImplementedException exception)
            {
                // not implemented operations return the 501 HTTP status code
                if (!await HandleExceptionAsync(httpContext, exception, StatusCodes.Status501NotImplemented, exception.Message).ConfigureAwait(false))
                    throw;
            }
            catch (Exception exception)
            {
                // all other unhandled exceptions return the 500 HTTP status code
                if (!await HandleExceptionAsync(httpContext, exception, StatusCodes.Status500InternalServerError, "Unhandled Error").ConfigureAwait(false))
                    throw;
            }
        }

        private async Task<bool> HandleExceptionAsync(HttpContext context, Exception exception, int status, string problemTitle)
        {
            // We can't do anything if the response has already started, just abort.
            if (context.Response.HasStarted)
                return false;

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var response = new ProblemDetails
            {
                Status = status,
                Title = problemTitle,
                Extensions = { ["Exception"] = exception },
            };
            var json = JsonConvert.SerializeObject(response, _settings);

            await context.Response.WriteAsync(json, context.RequestAborted).ConfigureAwait(false);

            return true;
        }

    }

    public class ProblemDetailsExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                const int statusCode = StatusCodes.Status501NotImplemented;
                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = ReasonPhrases.GetReasonPhrase(statusCode),
                };

                context.Result = new ObjectResult(problemDetails)
                {
                    StatusCode = statusCode,
                    ContentTypes =
                    {
                        "application/problem+json",
                        "application/problem+xml",
                    },
                };
            }
        }

    }
}