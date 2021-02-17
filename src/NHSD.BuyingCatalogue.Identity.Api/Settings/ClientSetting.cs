using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class ClientSetting
    {
        private const int OneHour = 3600;

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string AllowedGrantTypes { get; set; }

        /// <summary>
        /// Gets or sets the access token lifetime (in seconds).
        /// The default is one hour (3600 seconds).
        /// </summary>
        public int AccessTokenLifetime { get; set; } = OneHour;

        public bool AllowOfflineAccess { get; set; }

        public bool RequireClientSecret { get; set; }

        public bool RequirePkce { get; set; }

        public string Secret { get; set; }

        public bool RequireConsent { get; set; }

        public IEnumerable<string> RedirectUrls { get; set; }

        public IEnumerable<string> PostLogoutRedirectUrls { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }

        public IEnumerable<string> AllowedCorsOrigins { get; set; }

        public Client ToClient()
        {
            var allowedGrantTypes = AllowedGrantTypes switch
            {
                "ClientCredentials" => GrantTypes.ClientCredentials,
                "Code" => GrantTypes.Code,
                "Password" => GrantTypes.ResourceOwnerPassword,
                _ => new Client().AllowedGrantTypes,
            };

            return new Client
            {
                ClientId = ClientId,
                ClientName = ClientName,
                AllowOfflineAccess = AllowOfflineAccess,
                AllowedCorsOrigins = AllowedCorsOrigins?.ToList() ?? new List<string>(),
                RequireClientSecret = RequireClientSecret,
                RequirePkce = RequirePkce,
                ClientSecrets = new[] { new Secret(Secret?.ToSha256()) },
                RequireConsent = RequireConsent,
                RedirectUris = RedirectUrls?.ToList(),
                PostLogoutRedirectUris = PostLogoutRedirectUrls?.ToList(),
                AllowedScopes = AllowedScopes?.ToList(),
                AllowedGrantTypes = allowedGrantTypes,
                AccessTokenLifetime = AccessTokenLifetime,
            };
        }
    }
}
