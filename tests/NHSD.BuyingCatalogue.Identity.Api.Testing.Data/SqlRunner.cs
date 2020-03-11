using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data
{
    internal static class SqlRunner
    {
        internal static async Task ExecuteAsync<T>(string connectionString, string sql, T instance)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync(sql, instance);
        }
    }
}
