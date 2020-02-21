using System.Linq;
using IdentityServer4.Services;
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
            var loginUrl = Configuration.GetSection("loginUrl").Get<string>();
            var logoutUrl = Configuration.GetSection("logoutUrl").Get<string>();
            var errorUrl = Configuration.GetSection("errorUrl").Get<string>();
            
            var clientSection = Configuration.GetSection("clients");
            var resourceSection = Configuration.GetSection("resources");
            var identityResourceSection = Configuration.GetSection("identityResources");

            var clients = clientSection.Get<ClientSettingCollection>();
            var resources = resourceSection.Get<ApiResourceSettingCollection>();
            var identityResources = identityResourceSection.Get<IdentityResourceSettingCollection>();

            var builder = services.AddIdentityServer(options =>
                {
                    options.IssuerUri = issuerUrl;
                    options.UserInteraction.LoginUrl = loginUrl;
                    options.UserInteraction.LogoutUrl = logoutUrl;
                    options.UserInteraction.ErrorUrl = errorUrl;
                })
                .AddInMemoryIdentityResources(identityResources.Select(x => x.ToIdentityResource()))
                .AddInMemoryApiResources(resources.Select(x => x.ToResource()))
                .AddInMemoryClients(clients.Select(x => x.ToClient()));

            services.AddControllers();
            builder.AddDeveloperSigningCredential();
            services.AddTransient<IReturnUrlParser, Infrastructure.ReturnUrlParser>();
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
            app.UseRouting();
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
