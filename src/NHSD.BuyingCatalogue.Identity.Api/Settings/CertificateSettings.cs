using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    internal sealed class CertificateSettings
    {
        public bool UseDeveloperCredentials { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }

        public override string ToString()
        {
            var passMask = CertificatePassword.Substring(0, Math.Min(2, CertificatePassword.Length)) +
                           new string('*', Math.Max(0, CertificatePassword.Length - 2));
            return $"UseDeveloperCredentials: {UseDeveloperCredentials}, Path: {CertificatePath}, Pass: {passMask}";
        }
    }
}
