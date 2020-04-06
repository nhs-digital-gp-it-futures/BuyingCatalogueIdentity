using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public static class IntegrationDatabase
    {
        public static async Task ResetAsync(IConfiguration config)
        {
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("CatalogueUsersAdmin"));
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader ADD MEMBER NHSD;");
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter ADD MEMBER NHSD;");
            await databaseConnection.ExecuteAsync("DELETE FROM Organisations;");
            await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE [Email] NOT IN ('user@agency.com', 'AliceSmith@email.com', 'BobSmith@email.com');");
        }

        public static async Task RemoveReadRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader DROP MEMBER NHSD;");
        }

        public static async Task RemoveWriteRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter DROP MEMBER NHSD;");
        }
    }
}
