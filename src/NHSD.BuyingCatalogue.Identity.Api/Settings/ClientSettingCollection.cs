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
        public string AllowedGrantTypes { get; set; }
        public string Secret { get; set; }
        public IEnumerable<string> AllowedScopes { get; set; }

        public Client ToClient()
        {
            var client = new Client();
            client.ClientId = ClientId;
            client.AllowedGrantTypes = AllowedGrantTypes switch
            {
                "ClientCredentials" => GrantTypes.ClientCredentials,
                "Code" => GrantTypes.Code,
                _ => client.AllowedGrantTypes
            };
            client.ClientSecrets = new[] { new Secret(Secret.ToSha256()) };
            client.AllowedScopes = AllowedScopes.ToList();
            return client;
        }
    }
}
