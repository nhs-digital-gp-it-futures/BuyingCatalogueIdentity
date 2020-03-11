using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        internal static async Task<IEnumerable<T>> FetchAllAsync<T>(string connectionString, string selectSql,
            object param = null)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            return (await databaseConnection.QueryAsync<T>(selectSql, param).ConfigureAwait(false)).ToList();
        }
}
}
