using System;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Api.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseProblemDetails(this IApplicationBuilder app, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var args = jsonSerializerSettings != null
                ? new object[] { jsonSerializerSettings }
                : Array.Empty<object>();

            app.UseMiddleware<ProblemDetailsExceptionMiddleware>(args);

            return app;
        }

    }
}