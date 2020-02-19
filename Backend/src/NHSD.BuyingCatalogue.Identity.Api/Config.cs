using System.Collections.Generic;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("SampleResource", "Sample Resource")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "SampleClient",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("SampleClient".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "SampleResource" }
                }
            };
    }
}
