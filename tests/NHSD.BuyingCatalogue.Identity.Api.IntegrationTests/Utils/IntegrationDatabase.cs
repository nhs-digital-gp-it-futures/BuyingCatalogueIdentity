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

        public static async Task DropUser(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER DATABASE [CatalogueUsers] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
            await databaseConnection.ExecuteAsync("DROP USER NHSD;");
            await databaseConnection.ExecuteAsync("DROP LOGIN NHSD;");
        }

        public static async Task AddUser(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER DATABASE [CatalogueUsers] SET MULTI_USER;");
            await databaseConnection.ExecuteAsync("CREATE LOGIN NHSD WITH PASSWORD = 'DisruptTheMarket1!';");
            await databaseConnection.ExecuteAsync("CREATE USER NHSD FOR LOGIN NHSD;");
        }
    }
}
