using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string OrganisationApiBaseUrl { get; }

        public SmtpServerSettings Smtp { get; }

        public Settings(IConfigurationRoot config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            OrganisationApiBaseUrl = config.GetValue<string>("OrganisationApiBaseUrl");
            Smtp = config.GetSection("SmtpServer").Get<SmtpServerSettings>();
        }

        public sealed class SmtpServerSettings
        {

            public string Host { get; set; }

            public ushort Port { get; set; }

            public string ApiBaseUrl { get; set; }
        }
    }
}
