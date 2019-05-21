using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MMS.IdentityManagement.Api.Services;

namespace MMS.IdentityManagement.Api.Infrastructure
{
    public class ProblemDetailsExceptionFilter : IExceptionFilter
    {
        public virtual void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled) return;

            var serviceProvider = context.HttpContext.RequestServices;
            var problemDetailsFactory = serviceProvider.GetRequiredService<IProblemDetailsFactory>();
            var problemDetails = problemDetailsFactory.FromServerError(context.HttpContext, context.Exception);

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status,
                ContentTypes =
                {
                    "application/problem+json",
                    "application/problem+xml",
                },
            };

            context.ExceptionHandled = true;
        }

    }
}