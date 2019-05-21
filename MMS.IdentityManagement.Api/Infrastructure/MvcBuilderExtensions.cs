using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MMS.IdentityManagement.Api.Services;

namespace MMS.IdentityManagement.Api.Infrastructure
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddProblemDetails(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<IProblemDetailsFactory, ProblemDetailsFactory>();
            builder.Services.AddSingleton<IClientErrorFactory, ClientErrorFactory>();

            builder.AddMvcOptions(options =>
            {
                options.Filters.Add<ProblemDetailsExceptionFilter>();
            });

            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var serviceProvider = context.HttpContext.RequestServices;
                    var clientErrorFactory = serviceProvider.GetRequiredService<IClientErrorFactory>();
                    var result = clientErrorFactory.GetClientError(context, null);
                    return result;
                };
            });

            builder.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return builder;
        }

    }
}