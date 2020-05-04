using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            OrganisationsApiBaseUrl = config.GetValue<string>("OrganisationsApiBaseUrl");
            OdsApiWireMockBaseUrl = config.GetValue<string>("OdsApiWireMockBaseUrl");
        }

        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string OrganisationsApiBaseUrl { get; }

        public string OdsApiWireMockBaseUrl { get; }
    }
}
