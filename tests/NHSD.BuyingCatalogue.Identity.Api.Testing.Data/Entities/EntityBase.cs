using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public abstract class EntityBase
    {
        protected abstract string InsertSql { get; }

        protected abstract string GetSql { get; }

        public async Task InsertAsync(string connectionString) =>
            await SqlRunner.ExecuteAsync(connectionString, InsertSql, this);

        public async Task<GetUser> GetAsync(string connectionString) =>
            await SqlRunner.FetchSingleResultAsync<GetUser>(connectionString, GetSql, this);
    }
}
