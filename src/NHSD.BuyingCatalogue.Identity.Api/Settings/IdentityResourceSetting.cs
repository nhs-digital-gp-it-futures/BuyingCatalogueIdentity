using BuyingCatalogue.Identity.Common.Constants;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class IdentityResourceSetting
    {
        public string ResourceType { get; set; }

        public IdentityResource ToIdentityResource()
        {
            return ResourceType switch
            {
                "OpenId" => new IdentityResources.OpenId(),
                "Profile" => new CustomProfileIdentityResource(),
                "Email" => new IdentityResources.Email(),
                _ => (null as IdentityResource)
            };
        }

        private class CustomProfileIdentityResource : IdentityResources.Profile
        {
            public CustomProfileIdentityResource()
            {
                UserClaims.Add(ApplicationClaimTypes.PrimaryOrganisationId);
                UserClaims.Add(ApplicationClaimTypes.OrganisationFunction);
            }
        }
    }
}
