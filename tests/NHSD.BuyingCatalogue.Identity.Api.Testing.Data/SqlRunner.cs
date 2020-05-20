using System.Collections.Generic;
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

        internal static async Task ExecuteAsync<T>(string connectionString, string sql, IEnumerable<T> items)
        {
            await using var databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.ExecuteAsync(sql, items);
        }

        internal static async Task<T> FetchSingleResultAsync<T>(
            string connectionString,
            string selectSql,
            object param = null)
        {
            using IDbConnection databaseConnection = new SqlConnection(connectionString);
            return await databaseConnection.QueryFirstOrDefaultAsync<T>(selectSql, param);
        }

        internal static async Task<IEnumerable<dynamic>> QueryAsync(string connectionString, string sql)
        {
            await using var databaseConnection = new SqlConnection(connectionString);
            return await databaseConnection.QueryAsync(sql);
        }
    }
}
