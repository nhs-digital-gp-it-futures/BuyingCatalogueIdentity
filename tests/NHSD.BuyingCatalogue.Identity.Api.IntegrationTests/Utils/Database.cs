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
        public async Task Reset(IConfigurationRoot config, ScenarioContext context)
        {
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("CatalogueUsersAdmin"));
            await databaseConnection.ExecuteAsync("ALTER ROLE db_owner ADD MEMBER [NHSD];");
            await databaseConnection.ExecuteAsync("DELETE FROM Organisations");
            await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE OrganisationFunction = 'TestUser'");
            context["UserIds"] = new List<string>();
        }

        public static async Task DropUser(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("ALTER ROLE db_owner DROP MEMBER [NHSD];");
        }

        [AfterScenario]
        public async Task ClearUsers(IConfigurationRoot config, ScenarioContext context)
        {
            var userList = context["UserIds"] as List<string>;
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("CatalogueUsersAdmin"));
            foreach (var userId in userList)
            {
                await databaseConnection.ExecuteAsync("DELETE FROM AspNetUserClaims WHERE UserId = @userId", new { userId });
                await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE Id = @userId", new { userId });
            }
        }
    }
}
