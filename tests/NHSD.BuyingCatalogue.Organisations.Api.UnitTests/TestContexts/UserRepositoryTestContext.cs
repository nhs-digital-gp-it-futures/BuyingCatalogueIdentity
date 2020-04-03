using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Organisations.Api.Data;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts
{
    internal sealed class UserRepositoryTestContext
    {
        public ApplicationDbContext ContextInMemory { get; set; }
        public UsersRepository UserRepository { get; set; }

        private UserRepositoryTestContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Add_writes_to_db");
            ContextInMemory = new ApplicationDbContext(optionsBuilder.Options);

            UserRepository = new UsersRepository(ContextInMemory);
        }

        internal static UserRepositoryTestContext Setup()
        {
            return new UserRepositoryTestContext();
        }
    }
}
