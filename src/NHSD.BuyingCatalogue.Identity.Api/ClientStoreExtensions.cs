using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace NHSD.BuyingCatalogue.Identity.Api
{
    internal static class ClientStoreExtensions
    {
        internal static async Task<bool> IsPkceClientAsync(this IClientStore store, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return false;

            var client = await store.FindEnabledClientByIdAsync(clientId);
            return client?.RequirePkce == true;
        }
    }
}
