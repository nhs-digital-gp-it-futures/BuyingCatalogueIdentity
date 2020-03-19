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
            await databaseConnection.ExecuteAsync("ALTER ROLE db_owner ADD MEMBER [NHSD];");
            await databaseConnection.ExecuteAsync("DELETE FROM Organisations");
            await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE OrganisationFunction = 'TestUser'");
        }

        public static async Task DropUser(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_owner DROP MEMBER [NHSD];");
        }
    }
}
