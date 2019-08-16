using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MMS.IdentityManagement.Api.Data;
using MMS.IdentityManagement.Api.Infrastructure;
using MMS.IdentityManagement.Api.SecretProtectors;
using MMS.IdentityManagement.Api.Services;
using MMS.IdentityManagement.Data;
using MMS.IdentityManagement.Data.EntityFramework.Repositories;
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
            services.AddLogging();

            services.TryAddSingleton<ISecretProtectorSelector, SecretProtectorSelector>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISecretProtector, SecretProtectorBCrypt>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISecretProtector, SecretProtectorHmac256>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISecretProtector, SecretProtectorPbkdf2Sha256>());

            services.TryAddSingleton<ITokenService, TokenService>();
            services.TryAddSingleton<IClientService, ClientService>();
            services.TryAddSingleton<IClientValidator, ClientValidator>();
            services.TryAddSingleton<IKeyCodeValidator, KeyCodeValidator>();
            services.TryAddSingleton<IKeyCodeAuthenticationHandler, KeyCodeAuthenticationHandler>();

            services.TryAddSingleton<IClientRepository, ClientRepository>();
            services.TryAddSingleton<IMemberRepository, MemberRepository>();

            services
                .AddMvc()
                .AddProblemDetails();

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
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }

    }
}