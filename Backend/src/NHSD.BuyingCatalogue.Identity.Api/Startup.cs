using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using Serilog;
using NHSD.BuyingCatalogue.Identity.Api.Settings;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ASP.net needs this to not be static")]
    public sealed class Startup
    {
        private readonly IConfiguration Configuration;
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var issuerUrl = Configuration.GetSection("issuerUrl").Get<string>();
            var clientSection = Configuration.GetSection("clients");
            var resourceSection = Configuration.GetSection("resources");
            var identityResourceSection = Configuration.GetSection("identityResources");

            var clients = clientSection.Get<ClientSettingsCollection>();
            var resources = resourceSection.Get<ResourceSettingsCollection>();
            var identityResources = identityResourceSection.Get<IdentityResourceSettingsCollection>();

            var builder = services.AddIdentityServer(options => options.IssuerUri = issuerUrl)
                .AddInMemoryIdentityResources(identityResources.Select(x => x.ToIdentityResource()))
                .AddInMemoryApiResources(resources.Select(x => x.ToResource()))
                .AddInMemoryClients(clients.Select(x => x.ToClient()));

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
            }

            app.UseIdentityServer();
        }
    }
}
