using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    internal static class CustomIdentityServerBuilderExtensions
    {
        internal static IIdentityServerBuilder AddCustomSigningCredential(
            this IIdentityServerBuilder builder, CertificateSettings settings, ILogger logger)
        {
            logger = logger.ThrowIfNull(nameof(logger));
            settings = settings.ThrowIfNull(nameof(settings));

            if (settings.UseDeveloperCredentials)
            {
                logger.Information("Using Developer Signing Credential");
                return builder.AddDeveloperSigningCredential();
            }

            logger.Information("Using Certificate {certificatePath}", settings.CertificatePath);
            try
            {
#pragma warning disable CA2000 // Dispose objects before losing scope - deliberately not disposing this certificate, as it is used by identity server later on.
                var certificate = new X509Certificate2(settings.CertificatePath, settings.CertificatePassword);
#pragma warning restore CA2000 // Dispose objects before losing scope
                var verified = certificate.Verify();
                if (verified)
                    logger.Information("Certificate Verified: {certificate}", certificate.ToString(true));
                else
                    logger.Warning("Certificate Not Verified: {certificate}", certificate.ToString(true));

                return builder.AddSigningCredential(certificate);
            }
            catch (Exception e)
            {
                throw new CertificateSettingsException("Error in Certificate or Settings", e);
            }
        }
    }
}
