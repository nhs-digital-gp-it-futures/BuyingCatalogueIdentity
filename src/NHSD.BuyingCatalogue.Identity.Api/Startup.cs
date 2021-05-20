using System;
using System.Linq;
using System.Net.Http;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using NHSD.BuyingCatalogue.EmailClient.Configuration;
using NHSD.BuyingCatalogue.EmailClient.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Logging;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Api.Validators;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = configuration.GetConnectionString("CatalogueUsers");
            var cookieExpiration = configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();
            cookieExpiration.ConsentExpiration = configuration.GetValue<TimeSpan>(Cookies.BuyingCatalogueConsentExpiration);

            var clients = configuration.GetSection("clients").Get<ClientSettingCollection>();
            var apiResources = configuration.GetSection("resources").Get<ApiResourceSettingCollection>();

            var disabledErrorMessage = configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            var identityResources = configuration.GetSection("identityResources").Get<IdentityResourceSettingCollection>();
            var passwordResetSettings = configuration.GetSection("passwordReset").Get<PasswordResetSettings>();
            var dataProtectionAppName = configuration.GetValue<string>("dataProtection:applicationName");

            var allowInvalidCertificate = configuration.GetValue<bool>("AllowInvalidCertificate");
            var certificateSettings = configuration.GetSection("certificateSettings").Get<CertificateSettings>();
            var certificate = new Certificate(certificateSettings, Log.Logger);

            var smtpSettings = configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            smtpSettings.AllowInvalidCertificate ??= allowInvalidCertificate;

            var registrationSettings = configuration.GetSection("Registration").Get<RegistrationSettings>();

            var issuerUrl = configuration.GetValue<string>("issuerUrl");

            var issuerSettings = new IssuerSettings { IssuerUrl = new Uri(issuerUrl) };

            var publicBrowseSettings = configuration.GetSection("publicBrowse").Get<PublicBrowseSettings>();

            Log.Logger.Information("Clients: {@clients}", clients);
            Log.Logger.Information("Api Resources: {@resources}", apiResources);
            Log.Logger.Information("Identity Resources: {@identityResources}", identityResources);
            Log.Logger.Information("Issuer Url on IdentityAPI is: {@issuerUrl}", issuerUrl);
            Log.Logger.Information("Certificate Settings on IdentityAPI is: {@settings}", certificateSettings);
            Log.Logger.Information("Data protection app name is: {dataProtectionAppName}", dataProtectionAppName);
            Log.Logger.Information("Public Browse settings: {@publicBrowseSettings}", publicBrowseSettings);

            services.AddEmailClient(smtpSettings);

            services.AddSingleton(passwordResetSettings);
            services.AddSingleton(cookieExpiration);
            services.AddSingleton(disabledErrorMessage);
            services.AddSingleton(registrationSettings);
            services.AddSingleton(issuerSettings);
            services.AddSingleton<IScopeRepository>(new ScopeRepository(apiResources, identityResources));
            services.AddSingleton(publicBrowseSettings);

            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IOrganisationRepository, OrganisationRepository>();

            services
                .AddTransient<IRegistrationService, RegistrationService>()
                .AddTransient<ICreateBuyerService, CreateBuyerService>()
                .AddScoped<IAgreementConsentService, AgreementConsentService>()
                .AddScoped<ILoginService, LoginService>()
                .AddScoped<ILogoutService, LogoutService>()
                .AddScoped<IPasswordService, PasswordService>()
                .AddScoped<IPasswordResetCallback, PasswordResetCallback>();

            services.AddTransient<IApplicationUserValidator, ApplicationUserValidator>();

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
                .AddInMemoryIdentityResources(identityResources.Select(s => s.ToIdentityResource()))
                .AddInMemoryApiResources(apiResources.Select(s => s.ToResource()))
                .AddInMemoryClients(clients.Select(s => s.ToClient()))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>()
                .AddCustomSigningCredential(certificate, Log.Logger);

            services.AddTransient<IUserConsentStore, CatalogueAgreementConsentStore>();

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = cookieExpiration.ExpireTimeSpan;
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
            });

            services.AddHealthChecks(connectionString)
                .AddSmtpHealthCheck(smtpSettings);

            services.AddSwaggerDocumentation();

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
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
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

            if (environment.IsDevelopment())
                services.AddDatabaseDeveloperPageExceptionFilter();
        }

        public void Configure(IApplicationBuilder app)
        {
            var pathBase = configuration.GetValue<string>("pathBase");

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

            if (environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseSwaggerDocumentation();
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
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Live),
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready),
                });
            });
        }
    }
}
