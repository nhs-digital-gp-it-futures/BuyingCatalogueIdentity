using System;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    internal sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            BrokenSmtpIdentityApiBaseUrl = config.GetValue<Uri>("BrokenSmtpIdentityApiBaseUrl");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            DataProtectionAppName = config.GetValue<string>("DataProtectionAppName");
            IdentityApiBaseUrl = config.GetValue<Uri>("IdentityApiBaseUrl");
            PublicBrowseBaseUrl = config.GetValue<Uri>("PublicBrowseBaseUrl");
            PublicBrowseLoginUrl = config.GetValue<Uri>("PublicBrowseLoginUrl");
            PublicBrowseLogoutUrl = config.GetValue<Uri>("PublicBrowseLogoutUrl");
            SampleMvcClientBaseUrl = config.GetValue<Uri>("SampleMvcClientBaseUrl");
            SmtpServerApiBaseUrl = config.GetValue<Uri>("SmtpServerApiBaseUrl");
        }

        public string AdminConnectionString { get; }

        public Uri BrokenSmtpIdentityApiBaseUrl { get; }

        public string ConnectionString { get; }

        public string DataProtectionAppName { get; }

        public Uri IdentityApiBaseUrl { get; }

        public Uri PublicBrowseBaseUrl { get; }

        public Uri PublicBrowseLoginUrl { get; }

        public Uri PublicBrowseLogoutUrl { get; }

        public Uri SampleMvcClientBaseUrl { get; }

        public Uri SmtpServerApiBaseUrl { get; }
    }
}
