using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    public sealed class CertificateSettingsException : Exception
    {
        internal const string DefaultMessage = "An error occurred with the Certificate Settings.";

        public CertificateSettingsException(string message)
            : base(message)
        {
        }

        public CertificateSettingsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CertificateSettingsException()
            : this(DefaultMessage)
        {
        }
    }
}
