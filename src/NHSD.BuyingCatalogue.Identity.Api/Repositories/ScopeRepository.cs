using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.BuyingCatalogue.Identity.Api.Settings;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    internal sealed class ScopeRepository : IScopeRepository
    {
        private readonly List<string> _scopes = new List<string>();

        [SuppressMessage(
            "Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "OpenId scopes are lower case and must match")]
        public ScopeRepository(
            IEnumerable<ResourceSetting> apiResources,
            IEnumerable<IdentityResourceSetting> identityResources)
        {
            if (apiResources != null)
                _scopes.AddRange(apiResources.Select(r => r.ResourceName));

            if (identityResources != null)
                _scopes.AddRange(identityResources.Select(r => r.ResourceType.ToLowerInvariant()));
        }

        public IEnumerable<string> Scopes => _scopes;
    }
}
