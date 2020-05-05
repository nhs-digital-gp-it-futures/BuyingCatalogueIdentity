using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            DataProtectionAppName = config.GetValue<string>("DataProtectionAppName");
            IdentityApiBaseUrl = config.GetValue<string>("IdentityApiBaseUrl");
            BrokenSmtpIdentityApiBaseUrl = config.GetValue<string>("BrokenSmtpIdentityApiBaseUrl");
            SmtpServerApiBaseUrl = config.GetValue<string>("SmtpServerApiBaseUrl");
        }

        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string DataProtectionAppName { get; }

        public string IdentityApiBaseUrl { get; }

        public string BrokenSmtpIdentityApiBaseUrl { get; }

        public string SmtpServerApiBaseUrl { get; }
    }
}
