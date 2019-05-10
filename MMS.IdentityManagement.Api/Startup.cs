using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMS.IdentityManagement.Api.Infrastructure;
using MMS.IdentityManagement.Api.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace MMS.IdentityManagement.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<ITokenService, TokenService>();
            services.TryAddSingleton<ITokenConverter, TokenConverter>();

            services
                .AddMvc()
                .AddProblemDetails();

            services
                .AddAuthentication("KeyCode")
                .AddScheme<KeyCodeAuthenticationOptions, KeyCodeAuthenticationHandler>("KeyCode", options =>
                {
                    // nothing
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "MMS API",
                    Version = "v1"
                });
                options.DescribeAllEnumsAsStrings();
            });
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseProblemDetails();
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });
        }

    }

    public class KeyCodeAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public class KeyCodeAuthenticationHandler : AuthenticationHandler<KeyCodeAuthenticationOptions>
    {
        public KeyCodeAuthenticationHandler(IOptionsMonitor<KeyCodeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            // nothing
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }

}