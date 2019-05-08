using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Api
{
    public class ProblemDetailsExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _settings;
        private readonly IProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemDetailsExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate"/></param>
        /// <param name="problemDetailsFactory"><see cref="IProblemDetailsFactory"/></param>
        public ProblemDetailsExceptionMiddleware(RequestDelegate next, IProblemDetailsFactory problemDetailsFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemDetailsExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate"/></param>
        /// <param name="settings"><see cref="JsonSerializerSettings"/></param>
        /// <param name="problemDetailsFactory"><see cref="IProblemDetailsFactory"/></param>
        public ProblemDetailsExceptionMiddleware(RequestDelegate next, JsonSerializerSettings settings, IProblemDetailsFactory problemDetailsFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _settings = settings; // optional
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
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
            catch (Exception exception)
            {
                // we can't do anything if the response has already started
                if (httpContext.Response.HasStarted)
                    throw;

                var problemDetails = _problemDetailsFactory.FromServerError(httpContext, exception);
                var json = JsonConvert.SerializeObject(problemDetails, _settings);

                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/problem+json";

                await httpContext.Response.WriteAsync(json, httpContext.RequestAborted).ConfigureAwait(false);
            }
        }

    }
}