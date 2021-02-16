using System;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils
{
    public sealed class Config
    {
        public Config(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            OrganisationsApiBaseUrl = config.GetValue<Uri>("OrganisationsApiBaseUrl");
            OdsApiWireMockBaseUrl = config.GetValue<Uri>("OdsApiWireMockBaseUrl");
        }

        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public Uri OrganisationsApiBaseUrl { get; }

        public Uri OdsApiWireMockBaseUrl { get; }
    }
}
