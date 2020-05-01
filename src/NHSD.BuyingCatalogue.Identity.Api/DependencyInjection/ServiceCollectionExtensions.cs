using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Data;

namespace NHSD.BuyingCatalogue.Identity.Api.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        internal static void AddDataProtection(
            this IServiceCollection services,
            string applicationName,
            ICertificate certificate)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));

            if (string.IsNullOrWhiteSpace(applicationName))
                return;

            if (certificate.IsAvailable)
                AddDataProtection(services, applicationName, certificate.Instance);
            else
                AddDataProtection(services, applicationName);
        }

        private static void AddDataProtection(IServiceCollection services, string applicationName)
        {
            services.AddDataProtection()
                .SetApplicationName(applicationName)
                .PersistKeysToDbContext<ApplicationDbContext>();
        }

        private static void AddDataProtection(
            IServiceCollection services,
            string applicationName,
            X509Certificate2 certificate)
        {
            services.AddDataProtection()
                .SetApplicationName(applicationName)
                .PersistKeysToDbContext<ApplicationDbContext>()
                .ProtectKeysWithCertificate(certificate);
        }
    }
}
