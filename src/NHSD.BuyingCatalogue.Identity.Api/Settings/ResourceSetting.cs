using System.Collections.Generic;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class ResourceSetting
    {
        public string ResourceName { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<string> ClaimTypes { get; set; }

        public ApiResource ToResource()
        {
            return new(ResourceName, DisplayName, ClaimTypes);
        }
    }
}
