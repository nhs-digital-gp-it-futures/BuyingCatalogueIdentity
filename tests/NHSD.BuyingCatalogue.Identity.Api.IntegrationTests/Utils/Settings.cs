﻿using Microsoft.Extensions.Configuration;

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
            OrganisationApiBaseUrl = config.GetValue<string>("OrganisationApiBaseUrl");
            SmtpServerApiBaseUrl = config.GetValue<string>("SmtpServerApiBaseUrl");
            OdsApiWireMockBaseUrl = config.GetValue<string>("OdsApiWireMockBaseUrl");
        }

        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string DataProtectionAppName { get; }

        public string IdentityApiBaseUrl { get; }

        public string BrokenSmtpIdentityApiBaseUrl { get; }

        public string OrganisationApiBaseUrl { get; }

        public string SmtpServerApiBaseUrl { get; }

        public string OdsApiWireMockBaseUrl { get; }
    }
}
