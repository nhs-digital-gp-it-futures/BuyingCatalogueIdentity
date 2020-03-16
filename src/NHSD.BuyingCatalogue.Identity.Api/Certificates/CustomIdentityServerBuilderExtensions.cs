using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using Serilog;
using Serilog.Events;

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
                var certificate = new X509Certificate2(settings.CertificatePath, settings.CertificatePassword);
                var verified = certificate.Verify();
                if(verified)
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
