using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    [Binding]
    public sealed class Database
    {
        [BeforeScenario]
        public async Task Reset(IConfigurationRoot config)
        {
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("CatalogueUsersAdmin"));
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader ADD MEMBER NHSD;");
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter ADD MEMBER NHSD;");
            await databaseConnection.ExecuteAsync("DELETE FROM Organisations;");
            await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE SecurityStamp = 'TestUser';");
            await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE LockoutEnabled != 1;");
        }

        public static async Task RemoveReadRole(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader DROP MEMBER NHSD;");
        }

        public static async Task RemoveWriteRole(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter DROP MEMBER NHSD;");
        }
    }
}
