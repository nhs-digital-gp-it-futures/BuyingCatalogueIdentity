using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Organisations.Api.Data;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
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
            services.AddTransient<IOrganisationRepository, OrganisationRepository>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CatalogueUsers")));

            var authority = Configuration.GetValue<string>("authority");
            var requireHttps = Configuration.GetValue<bool>("RequireHttps");

            services.AddAuthentication(BearerToken)
                .AddJwtBearer(BearerToken, options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = requireHttps;
                    options.Audience = "Organisation";
                });

            services.AddControllers();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.OrganisationPolicy, policy => policy.RequireClaim("Organisation"));
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
            });

            foreach (IConfigurationSection configurationSection in Configuration.GetChildren())
            {
                logger.LogInformation("{Key} = {Value}", configurationSection.Key, configurationSection.Value);
            }
        }
    }
}
