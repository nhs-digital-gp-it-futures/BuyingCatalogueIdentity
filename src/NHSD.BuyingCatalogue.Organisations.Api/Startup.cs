using System;
using System.Net.Http;
using HealthChecks.Network.Core;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Organisations.Api.Data;
using NHSD.BuyingCatalogue.Organisations.Api.Extensions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;
using Serilog;

namespace NHSD.BuyingCatalogue.Organisations.Api
{
    public class Startup
    {
        private const string BearerToken = "Bearer";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("CatalogueUsers");
            var authority = Configuration.GetValue<string>("authority");
            var requireHttps = Configuration.GetValue<bool>("RequireHttps");
            var allowInvalidCertificate = Configuration.GetValue<bool>("AllowInvalidCertificate");
            var registrationSettings = Configuration.GetSection("Registration").Get<RegistrationSettings>();

            var smtpSettings = Configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            if (!smtpSettings.AllowInvalidCertificate.HasValue)
                smtpSettings.AllowInvalidCertificate = allowInvalidCertificate;

            services.AddTransient<IOrganisationRepository, OrganisationRepository>();
            services.AddTransient<IUsersRepository, UsersRepository>();

            services.AddTransient<IApplicationUserValidator, ApplicationUserValidator>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton(registrationSettings);
            services.AddSingleton(smtpSettings);
            services.AddScoped<IMailTransport, SmtpClient>();

            services.AddTransient<IEmailService, MailKitEmailService>()
                .AddTransient<IRegistrationService, RegistrationService>()
                .AddTransient<ICreateBuyerService, CreateBuyerService>();

            services.RegisterHealthChecks(connectionString, smtpSettings);

            services.AddAuthentication(BearerToken)
                .AddJwtBearer(BearerToken, options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = requireHttps;
                    options.Audience = "Organisation";
                    if (allowInvalidCertificate)
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                    }
                });

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.CanAccessOrganisation, policy => policy.RequireClaim(ApplicationClaimTypes.Organisation));
                options.AddPolicy(Policy.CanAccessOrganisationUsers, policyBuilder =>
                {
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Organisation);
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Account);
                });
                options.AddPolicy(Policy.CanManageOrganisationUsers, policyBuilder =>
                {
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Organisation, ApplicationPermissions.Manage);
                    policyBuilder.RequireClaim(ApplicationClaimTypes.Account, ApplicationPermissions.Manage);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
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

            foreach (IConfigurationSection configurationSection in Configuration.GetChildren())
            {
                logger.LogInformation("{Key} = {Value}", configurationSection.Key, configurationSection.Value);
            }
        }
    }
}
