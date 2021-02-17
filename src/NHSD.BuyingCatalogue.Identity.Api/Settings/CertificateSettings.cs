namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    internal sealed class CertificateSettings
    {
        public bool UseDeveloperCredentials { get; set; }

        public string CertificatePath { get; set; }

        public string PrivateKeyPath { get; set; }
    }
}
