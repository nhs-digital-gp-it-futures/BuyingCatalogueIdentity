using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class ClientSettingCollection : List<ClientSetting>
    {
    }

    public sealed class ClientSetting
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string AllowedGrantTypes { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool RequireClientSecret { get; set; }
        public string Secret { get; set; }
        public bool RequireConsent { get; set; }
        public IEnumerable<string> RedirectUrls { get; set; }
        public IEnumerable<string> PostLogoutRedirectUrls { get; set; }
        public IEnumerable<string> AllowedScopes { get; set; }

        public Client ToClient()
        {
            var allowedGrantTypes = AllowedGrantTypes switch
            {
                "ClientCredentials" => GrantTypes.ClientCredentials,
                "Code" => GrantTypes.Code,
                _ => new Client().AllowedGrantTypes
            };

            return new Client
            {
                ClientId = ClientId,
                ClientName = ClientName,
                AllowOfflineAccess = AllowOfflineAccess,
                RequireClientSecret = RequireClientSecret,
                ClientSecrets = new[] {new Secret(Secret?.ToSha256())},
                RequireConsent = RequireConsent,
                RedirectUris = RedirectUrls?.ToList(),
                PostLogoutRedirectUris = PostLogoutRedirectUrls?.ToList(),
                AllowedScopes = AllowedScopes?.ToList(),
                AllowedGrantTypes = allowedGrantTypes,
                RequirePkce = true
            };
        }
    }
}
