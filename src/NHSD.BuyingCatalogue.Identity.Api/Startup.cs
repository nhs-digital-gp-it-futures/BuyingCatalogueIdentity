using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using IdentityServer4.Stores;
using MailKit;
using MailKit.Net.Smtp;
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
using NHSD.BuyingCatalogue.Identity.Api.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Logging;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Api.Validators;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Email;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.Settings;
using Serilog;

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
            var apiResources = _configuration.GetSection("resources").Get<ApiResourceSettingCollection>();

            var disabledErrorMessage = _configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            var identityResources = _configuration.GetSection("identityResources").Get<IdentityResourceSettingCollection>();
            var passwordResetSettings = _configuration.GetSection("passwordReset").Get<PasswordResetSettings>();
            var dataProtectionAppName = _configuration.GetValue<string>("dataProtection:applicationName");

            var allowInvalidCertificate = _configuration.GetValue<bool>("AllowInvalidCertificate");
            var certificateSettings = _configuration.GetSection("certificateSettings").Get<CertificateSettings>();
            var certificate = new Certificate(certificateSettings, Log.Logger);

            var smtpSettings = _configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            if (!smtpSettings.AllowInvalidCertificate.HasValue)
                smtpSettings.AllowInvalidCertificate = allowInvalidCertificate;

            var registrationSettings = _configuration.GetSection("Registration").Get<RegistrationSettings>();

            var issuerUrl = _configuration.GetValue<string>("issuerUrl");

            var publicBrowseSettings = _configuration.GetSection("publicBrowse").Get<PublicBrowseSettings>();

            Log.Logger.Information("Clients: {@clients}", clients);
            Log.Logger.Information("Api Resources: {@resources}", apiResources);
            Log.Logger.Information("Identity Resources: {@identityResources}", identityResources);
            Log.Logger.Information("Issuer Url on IdentityAPI is: {@issuerUrl}", issuerUrl);
            Log.Logger.Information("Certificate Settings on IdentityAPI is: {settings}", certificateSettings);
            Log.Logger.Information("Data protection app name is: {dataProtectionAppName}", dataProtectionAppName);
            Log.Logger.Information("Public Browse settings: {@publicBrowseSettings}", publicBrowseSettings);

            services.AddSingleton(passwordResetSettings);
            services.AddSingleton(smtpSettings);
            services.AddSingleton(cookieExpiration);
            services.AddSingleton(disabledErrorMessage);
            services.AddSingleton(registrationSettings);
            services.AddSingleton<IScopeRepository>(new ScopeRepository(apiResources, identityResources));
            services.AddSingleton(publicBrowseSettings);

            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IOrganisationRepository, OrganisationRepository>();

            services
                .AddTransient<IRegistrationService, RegistrationService>()
                .AddTransient<ICreateBuyerService, CreateBuyerService>()
                .AddTransient<IEmailService, MailKitEmailService>()
                .AddScoped<IAgreementConsentService, AgreementConsentService>()
                .AddScoped<ILoginService, LoginService>()
                .AddScoped<ILogoutService, LogoutService>()
                .AddScoped<IPasswordService, PasswordService>()
                .AddScoped<IPasswordResetCallback, PasswordResetCallback>();

            services.AddTransient<IApplicationUserValidator, ApplicationUserValidator>();

            services.AddScoped<IMailTransport, SmtpClient>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options =>
                    {
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 10;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider)
                .AddPasswordValidator<PasswordValidator>();

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
                .AddInMemoryApiResources(apiResources.Select(x => x.ToResource()))
                .AddInMemoryClients(clients.Select(x => x.ToClient()))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>()
                .AddCustomSigningCredential(certificate, Log.Logger);

            services.AddTransient<IUserConsentStore, CatalogueAgreementConsentStore>();

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = cookieExpiration.ExpireTimeSpan;
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
            });

            services.RegisterHealthChecks(connectionString, smtpSettings);

            services.AddAuthentication()
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = issuerUrl;
                    options.ApiName = "Organisation";
                    options.RequireHttpsMetadata = false;

                    if (allowInvalidCertificate)
                    {
                        options.JwtBackChannelHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    }
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyName.CanAccessOrganisationUsers, policyBuilder =>
                {
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Organisation);
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Account);
                });
                options.AddPolicy(PolicyName.CanManageOrganisationUsers, policyBuilder =>
                {
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Organisation, ApplicationPermissions.Manage);
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Account, ApplicationPermissions.Manage);
                });
            });

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddControllersWithViews();

            services.AddDataProtection(dataProtectionAppName, certificate);
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
                app.Map($"/{pathBase}", ConfigureApp);
            }
        }

        public void ConfigureApp(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = SerilogRequestLoggingOptions.EnrichFromRequest;
                opts.GetLevel = SerilogRequestLoggingOptions.GetLevel;
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
