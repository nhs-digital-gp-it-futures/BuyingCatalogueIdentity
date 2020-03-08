using System.Collections.Generic;
using IdentityServer4.Models;
using NHSD.BuyingCatalogue.Identity.Api.Constants;

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
                "Profile" => new IdentityResources.Profile(),
                "Email" => new IdentityResources.Email(),
                "Organisation" => new OrganisationIdentityResource(),
                _ => (null as IdentityResource)
            };
        }

        private sealed class OrganisationIdentityResource : IdentityResource
        {
            public OrganisationIdentityResource()
            {
                Name = "organisation";
                DisplayName = "Organisation profile";
                Description = "Your organisation profile information (primary organisation, organisation function)";
                UserClaims = new List<string>
                {
                    CustomClaimTypes.PrimaryOrganisationId,
                    CustomClaimTypes.OrganisationFunction
                };
            }
        }
    }
}
