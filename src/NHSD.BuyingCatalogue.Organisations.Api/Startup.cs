using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Organisations.Api.Data;
using NHSD.BuyingCatalogue.Organisations.Api.Extensions;
using NHSD.BuyingCatalogue.Organisations.Api.Logging;
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
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("CatalogueUsers");
            var authority = Configuration.GetValue<string>("authority");
            var requireHttps = Configuration.GetValue<bool>("RequireHttps");
            var odsSettings = Configuration.GetSection("Ods").Get<OdsSettings>();
            var allowInvalidCertificate = Configuration.GetValue<bool>("AllowInvalidCertificate");

            IdentityModelEventSource.ShowPII = _environment.IsDevelopment();

            services.AddTransient<IOrganisationRepository, OrganisationRepository>();
            services.AddTransient<IOdsRepository, OdsRepository>();
            services.AddTransient<IServiceRecipientRepository, ServiceRecipientRepository>();

            services.AddTransient<ICreateOrganisationService, CreateOrganisationService>();

            services.AddTransient<IOrganisationValidator, OrganisationValidator>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton(odsSettings);

            services.AddHealthChecks(connectionString);

            services.AddSwaggerDocumentation();

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
                options.AddPolicy(PolicyName.CanAccessOrganisations, policy => policy.RequireClaim(ApplicationClaimTypes.Organisation));
                options.AddPolicy(PolicyName.CanManageOrganisations, policy =>
                    policy.RequireClaim(ApplicationClaimTypes.Organisation, ApplicationPermissions.Manage));

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = SerilogRequestLoggingOptions.GetLevel;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerDocumentation();
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
