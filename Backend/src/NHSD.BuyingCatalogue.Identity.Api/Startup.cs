using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    public sealed class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer(options => options.IssuerUri = "http://localhost:8070")
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients);

            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.GetLevel = LogHelper.ExcludeHealthChecks;
            });

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger().UseSwaggerUI(options =>
                //{
                //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Buying Catalogue Identity Service V1");
                //});
            }

            app.UseIdentityServer();
        }
    }
}
