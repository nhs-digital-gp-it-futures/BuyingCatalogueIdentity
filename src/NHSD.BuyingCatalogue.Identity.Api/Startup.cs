using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ASP.net needs this to not be static")]
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var clients = _configuration.GetSection("clients").Get<ClientSettingCollection>();
            var resources = _configuration.GetSection("resources").Get<ApiResourceSettingCollection>();
            var identityResources = _configuration.GetSection("identityResources").Get<IdentityResourceSettingCollection>();

            Log.Logger.Information("Clients: {@clients}", clients);
            Log.Logger.Information("Api Resources: {@resources}", resources);
            Log.Logger.Information("Identity Resources: {@identityResources}", identityResources);

            services.AddScoped<ILogoutService, LogoutService>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("CatalogueUsers")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.IssuerUri = _configuration.GetValue<string>("issuerUrl");
                })
            .AddInMemoryIdentityResources(identityResources.Select(x => x.ToIdentityResource()))
            .AddInMemoryApiResources(resources.Select(x => x.ToResource()))
            .AddInMemoryClients(clients.Select(x => x.ToClient()))
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential();

            services.AddTransient<IOrganisationRepository, OrganisationRepository>();

            services.AddControllers();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = Infrastructure.LogHelper.EnrichFromRequest;
                opts.GetLevel = Infrastructure.LogHelper.ExcludeHealthChecks;
            });

            if (_environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Account/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }
}
