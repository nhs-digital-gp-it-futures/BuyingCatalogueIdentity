using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string IdentityApiBaseUrl { get; }
        public string BrokenIdentityApiBaseUrl { get; }
        public string OrganisationApiBaseUrl { get; }

        public string BrokenDbOrganisationApiBaseUrl { get; }

        public string BrokenSmtpOrganisationApiBaseUrl { get; }

        public string SmtpServerApiBaseUrl { get; }
        
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            IdentityApiBaseUrl = config.GetValue<string>("IdentityApiBaseUrl");
            BrokenIdentityApiBaseUrl = config.GetValue<string>("BrokenIdentityApiBaseUrl");
            OrganisationApiBaseUrl = config.GetValue<string>("OrganisationApiBaseUrl");
            BrokenDbOrganisationApiBaseUrl = config.GetValue<string>("BrokenDbOrganisationApiBaseUrl");
            BrokenSmtpOrganisationApiBaseUrl = config.GetValue<string>("BrokenSmtpOrganisationApiBaseUrl");
            SmtpServerApiBaseUrl = config.GetValue<string>("SmtpServerApiBaseUrl");
        }
    }
}
