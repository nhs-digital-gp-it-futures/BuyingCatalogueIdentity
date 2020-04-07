using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string IdentityApiBaseUrl { get; }

        public string BrokenSmtpIdentityApiBaseUrl { get; }

        public string OrganisationApiBaseUrl { get; }

        public string BrokenSmtpOrganisationApiBaseUrl { get; }

        public string SmtpServerApiBaseUrl { get; }
        
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            IdentityApiBaseUrl = config.GetValue<string>("IdentityApiBaseUrl");
            BrokenSmtpIdentityApiBaseUrl = config.GetValue<string>("BrokenSmtpIdentityApiBaseUrl");
            OrganisationApiBaseUrl = config.GetValue<string>("OrganisationApiBaseUrl");
            BrokenSmtpOrganisationApiBaseUrl = config.GetValue<string>("BrokenSmtpOrganisationApiBaseUrl");
            SmtpServerApiBaseUrl = config.GetValue<string>("SmtpServerApiBaseUrl");
        }
    }
}
