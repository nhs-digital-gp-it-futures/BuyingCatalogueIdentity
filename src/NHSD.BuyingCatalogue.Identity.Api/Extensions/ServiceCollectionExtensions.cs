using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterHealthChecks(this IServiceCollection services, string connectionString)
        {
            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            services.AddHealthChecks()
                .AddCheck(
                    "self",
                    () => HealthCheckResult.Healthy(),
                    new[] {HealthCheckTags.Live})
                .AddSqlServer(
                    connectionString,
                    "SELECT 1;",
                    "db",
                    HealthStatus.Unhealthy,
                    new[] {HealthCheckTags.Ready},
                    TimeSpan.FromSeconds(10));

            return services;
        }
    }
}
