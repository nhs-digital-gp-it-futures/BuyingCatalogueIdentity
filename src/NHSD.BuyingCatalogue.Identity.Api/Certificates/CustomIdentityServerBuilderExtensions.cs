using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using Serilog;
using Serilog.Events;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    public static class CustomIdentityServerBuilderExtensions
    {
        private const string LogTemplate =
        @"Certificate Details: 
        Content Type: {contentType}
        Friendly Name: {friendlyName}
        SUbject: {subject}
        Signature Algorithm { signatureAlgorithm }
        Private Key: {privateKey}
        Public Key: {publicKey}
        Archived: {archived}
        Verified: {verified}";

        public static IIdentityServerBuilder AddCustomSigningCredential(
            this IIdentityServerBuilder builder, CertificateSettings settings, ILogger logger)
        {
            logger = logger.ThrowIfNull();
            settings = settings.ThrowIfNull();
            if (settings.UseDeveloperCredentials)
            {
                logger.Information("Using Developer Signing Credential");
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                logger.Information("Using Certificate {certificatePath}", settings.CertificatePath);
                try
                {
                    var certificate = new X509Certificate2(settings.CertificatePath, settings.CertificatePassword);
                    var verified = certificate.Verify();
                    logger.Write(verified ? LogEventLevel.Information : LogEventLevel.Warning,
                        LogTemplate,
                        X509Certificate2.GetCertContentType(certificate.RawData),
                        certificate.FriendlyName,
                        certificate.Subject,
                        certificate.SignatureAlgorithm.FriendlyName,
                        certificate.PrivateKey.ToXmlString(false),
                        certificate.PublicKey.Key.ToXmlString(false),
                        certificate.Archived,
                        verified);
                    
                    builder.AddSigningCredential(certificate);
                }
                catch (Exception e)
                {
                    throw new CertificateSettingsException("Error in Certificate or Settings", e);
                }
            }

            return builder;
        }
    }
}
