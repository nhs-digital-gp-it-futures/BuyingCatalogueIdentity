using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string OrganisationApiBaseUrl { get; }

        public string BrokenOrganisationApiBaseUrl { get; }

        public Settings(IConfigurationRoot config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            OrganisationApiBaseUrl = config.GetValue<string>("OrganisationApiBaseUrl");
            BrokenOrganisationApiBaseUrl = config.GetValue<string>("BrokenOrganisationApiBaseUrl");
        }
    }
}
