using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public static class DataProtectionKeys
    {
        public static async Task<IReadOnlyList<DataProtectionKey>> GetFromDbAsync(string connectionString)
        {
            const string sql = "SELECT FriendlyName, Xml FROM dbo.DataProtectionKeys;";

            await using var databaseConnection = new SqlConnection(connectionString);
            var keys = await databaseConnection.QueryAsync(sql);

            return keys.Select(k => new DataProtectionKey((string)k.Xml)).ToList();
        }

        public static async Task SaveToDbAsync(string connectionString, IEnumerable<DataProtectionKey> keys)
        {
            const string sql = "INSERT INTO dbo.DataProtectionKeys(FriendlyName, [Xml]) VALUES (@FriendlyName, @Xml);";

            var existingDbKeys = await GetFromDbAsync(connectionString);
            var newKeys = keys.Except(existingDbKeys);

            await using var databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync(sql, newKeys);
        }
    }
}
