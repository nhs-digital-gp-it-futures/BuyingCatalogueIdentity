using System;
using System.IO;
using System.Security.Cryptography;
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
                byte[] certBuffer = GetBytesFromPem(settings.CertificatePath, "CERTIFICATE");
                byte[] keyBuffer = GetBytesFromPem(settings.PrivateKeyPath, "RSA PRIVATE KEY");

                using var certificate = new X509Certificate2(certBuffer);
                using var rsa = RSA.Create();
                
                rsa.ImportRSAPrivateKey(keyBuffer, out _);
                var certificateWithPrivateKey = certificate.CopyWithPrivateKey(rsa);

                if (certificateWithPrivateKey.Verify())
                    logger.Information("Certificate validation succeeded: {certificate}", certificate.ToString(true));
                else
                    logger.Warning("Certificate validation failed: {certificate}", certificate.ToString(true));

                return certificateWithPrivateKey;
            }
            catch (Exception e)
            {
                throw new CertificateSettingsException("Error with certificate or settings", e);
            }
        }
        private static byte[] GetBytesFromPem(string pemString, string sectionToken)
        {
            var fileStr = File.ReadAllText(pemString);
            string header = $"-----BEGIN {sectionToken}-----"; 
            string footer = $"-----END {sectionToken}-----";

            int start = fileStr.IndexOf(header, StringComparison.InvariantCultureIgnoreCase) + header.Length;
            int end = fileStr.IndexOf(footer, start, StringComparison.InvariantCultureIgnoreCase) - start;
            return Convert.FromBase64String(fileStr.Substring(start, end));
        }

    }
}
