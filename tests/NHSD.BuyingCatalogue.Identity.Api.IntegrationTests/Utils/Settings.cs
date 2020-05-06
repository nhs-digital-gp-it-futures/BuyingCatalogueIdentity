using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            BrokenSmtpIdentityApiBaseUrl = config.GetValue<string>("BrokenSmtpIdentityApiBaseUrl");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            DataProtectionAppName = config.GetValue<string>("DataProtectionAppName");
            IdentityApiBaseUrl = config.GetValue<string>("IdentityApiBaseUrl");
            PublicBrowseBaseUrl = config.GetValue<string>("PublicBrowseBaseUrl");
            PublicBrowseLoginUrl = config.GetValue<string>("PublicBrowseLoginUrl");
            SampleMvcClientBaseUrl = config.GetValue<string>("SampleMvcClientBaseUrl");
            SmtpServerApiBaseUrl = config.GetValue<string>("SmtpServerApiBaseUrl");
        }

        public string AdminConnectionString { get; }

        public string BrokenSmtpIdentityApiBaseUrl { get; }

        public string ConnectionString { get; }

        public string DataProtectionAppName { get; }

        public string IdentityApiBaseUrl { get; }

        public string PublicBrowseBaseUrl { get; }

        public string PublicBrowseLoginUrl { get; }

        public string SampleMvcClientBaseUrl { get; }

        public string SmtpServerApiBaseUrl { get; }
    }
}
