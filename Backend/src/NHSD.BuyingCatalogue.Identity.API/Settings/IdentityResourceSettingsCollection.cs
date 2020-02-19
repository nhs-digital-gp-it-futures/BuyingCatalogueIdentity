using System.Collections.Generic;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class IdentityResourceSettingsCollection : List<IdentityResourceSetting>
    {
    }

    public sealed class IdentityResourceSetting
    {
        public string ResourceType { get; set; }

        public IdentityResource ToIdentityResource()
        {
            return ResourceType switch
            {
                "OpenId" => new IdentityResources.OpenId(),
                "Profile" => new IdentityResources.Profile(),
                "Email" => new IdentityResources.Email(),
                _ => (IdentityResource)null
            };
        }
    }
}
