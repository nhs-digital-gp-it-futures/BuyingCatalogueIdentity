using System;
using System.Security.Cryptography.X509Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    internal sealed class Certificate : ICertificate
    {
        internal Certificate(CertificateSettings certificateSettings, ILogger logger)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            if (certificateSettings is null)
                throw new ArgumentNullException(nameof(certificateSettings));

            Path = certificateSettings.CertificatePath;
            Instance = Load(logger, certificateSettings);
        }

        public X509Certificate2 Instance { get; }

        public bool IsAvailable => Instance != null;

        public string Path { get; }

        private static X509Certificate2 Load(ILogger logger, CertificateSettings settings)
        {
            if (settings.UseDeveloperCredentials)
                return null;

            var path = settings.CertificatePath;
            logger.Information("Loading certificate {path}", path);

            try
            {
                var certificate = new X509Certificate2(path, settings.CertificatePassword);

                if (certificate.Verify())
                    logger.Information("Certificate validation succeeded: {certificate}", certificate.ToString(true));
                else
                    logger.Warning("Certificate validation failed: {certificate}", certificate.ToString(true));

                return certificate;
            }
            catch (Exception e)
            {
                throw new CertificateSettingsException("Error with certificate or settings", e);
            }
        }
    }
}
