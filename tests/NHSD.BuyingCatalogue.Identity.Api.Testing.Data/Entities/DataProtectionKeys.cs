using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public static class DataProtectionKeys
    {
        public static async Task<IReadOnlyCollection<DataProtectionKey>> GetFromDbAsync(string connectionString)
        {
            const string sql = "SELECT FriendlyName, Xml FROM dbo.DataProtectionKeys;";

            IEnumerable<dynamic> keys = await SqlRunner.QueryAsync(connectionString, sql);

            return keys.Select(k => new DataProtectionKey((string)k.Xml)).ToList();
        }

        public static async Task SaveToDbAsync(string connectionString, IEnumerable<DataProtectionKey> keys)
        {
            const string sql = "INSERT INTO dbo.DataProtectionKeys(FriendlyName, [Xml]) VALUES (@FriendlyName, @Xml);";

            var existingDbKeys = await GetFromDbAsync(connectionString);
            var newKeys = keys.Except(existingDbKeys);

            await SqlRunner.ExecuteAsync(connectionString, sql, newKeys);
        }
    }
}
