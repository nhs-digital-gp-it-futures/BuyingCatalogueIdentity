using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using Serilog;
using LogHelper = NHSD.BuyingCatalogue.Identity.Api.Infrastructure.LogHelper;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    [SuppressMessage("Performance", "CA1822:Mark members as static",
        Justification = "ASP.net needs this to not be static")]
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
            var connectionString = _configuration.GetConnectionString("CatalogueUsers");
            var cookieExpiration = _configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();
            var clients = _configuration.GetSection("clients").Get<ClientSettingCollection>();
            var resources = _configuration.GetSection("resources").Get<ApiResourceSettingCollection>();
            var identityResources =
                _configuration.GetSection("identityResources").Get<IdentityResourceSettingCollection>();
            var certificateSettings = _configuration.GetSection("certificateSettings").Get<CertificateSettings>();

            var issuerUrl = _configuration.GetValue<string>("issuerUrl");

            Log.Logger.Information("Clients: {@clients}", clients);
            Log.Logger.Information("Api Resources: {@resources}", resources);
            Log.Logger.Information("Identity Resources: {@identityResources}", identityResources);
            Log.Logger.Information("Issuer Url on IdentityAPI is: {@issuerUrl}", issuerUrl);
            Log.Logger.Information("Certificate Settings on IdentityAPI is: {settings}", certificateSettings);

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILogoutService, LogoutService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.IssuerUri = issuerUrl;
                    options.UserInteraction.ErrorUrl = "/Error";
                    options.UserInteraction.ErrorIdParameter = "errorId";
                })
                .AddInMemoryIdentityResources(identityResources.Select(x => x.ToIdentityResource()))
                .AddInMemoryApiResources(resources.Select(x => x.ToResource()))
                .AddInMemoryClients(clients.Select(x => x.ToClient()))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>()
                .AddCustomSigningCredential(certificateSettings, Log.Logger);

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = cookieExpiration.ExpireTimeSpan;
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
            });

            services.RegisterHealthChecks(connectionString);
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app)
        {
            var pathBase = _configuration.GetValue<string>("pathBase");

            if (string.IsNullOrWhiteSpace(pathBase))
            {
                ConfigureApp(app);
            }
            else
            {
                app.Map($"/{pathBase}", mappedApp =>
                {
                    ConfigureApp(mappedApp);
                });
            }
        }
        public void ConfigureApp(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.GetLevel = LogHelper.ExcludeHealthChecks;
            });

            if (_environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Live)
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready)
                });
            });
        }
    }
}
