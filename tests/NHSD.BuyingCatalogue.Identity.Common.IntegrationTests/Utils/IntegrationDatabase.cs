﻿using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils
{
    internal static class IntegrationDatabase
    {
        public static async Task ResetAsync(IConfiguration config)
        {
            using IDbConnection databaseConnection = new SqlConnection(config.GetConnectionString("CatalogueUsersAdmin"));
            await databaseConnection.ExecuteAsync("GRANT CONNECT TO [NHSD-ISAPI];");

            // ReSharper disable StringLiteralTypo
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader ADD MEMBER [NHSD-ISAPI];");
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter ADD MEMBER [NHSD-ISAPI];");

            // ReSharper restore StringLiteralTypo
            await databaseConnection.ExecuteAsync("DELETE FROM RelatedOrganisations;");
            await databaseConnection.ExecuteAsync("DELETE FROM Organisations;");
            await databaseConnection.ExecuteAsync("DELETE FROM AspNetUsers WHERE [Email] NOT IN ('user@agency.com', 'AliceSmith@email.com', 'BobSmith@email.com', 'SueSmith@email.com');");
        }

        public static async Task RemoveReadRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);

            // ReSharper disable once StringLiteralTypo
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datareader DROP MEMBER [NHSD-ISAPI];");
        }

        public static async Task RemoveWriteRoleAsync(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);

            // ReSharper disable once StringLiteralTypo
            await databaseConnection.ExecuteAsync("ALTER ROLE db_datawriter DROP MEMBER [NHSD-ISAPI];");
        }

        public static async Task DenyAccessForNhsdUser(string connectionString)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync("DENY CONNECT TO [NHSD-ISAPI];");
        }
    }
}
