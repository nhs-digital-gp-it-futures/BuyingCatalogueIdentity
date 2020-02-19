using System.Collections.Generic;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class ResourceSettingsCollection : List<ResourceSetting>
    {
    }

    public sealed class ResourceSetting
    {
        public string ResourceName { get; set; }

        public string DisplayName { get; set; }

        public ApiResource ToResource()
        {
            return new ApiResource(ResourceName, DisplayName);
        }
    }
}
