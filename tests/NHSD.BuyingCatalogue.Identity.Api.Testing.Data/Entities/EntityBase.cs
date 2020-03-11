using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public abstract class EntityBase
    {
        protected abstract string InsertSql { get; }

        public async Task InsertAsync(string connectionString) =>
            await SqlRunner.ExecuteAsync(connectionString, InsertSql, this);
    }
}
