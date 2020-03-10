using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    public class CertificateSettingsException : Exception
    {
        public CertificateSettingsException(string message) : base(message)
        {
        }

        public CertificateSettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CertificateSettingsException() : this("Certificate Settings Incorrect")
        {
        }
    }
}
