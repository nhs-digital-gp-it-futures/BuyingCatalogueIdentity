using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Database
    {
        public static void ResetUser(string config)
        {
            using var context = GetContext(config);

            context.Database.ExecuteSqlRaw("ALTER ROLE db_owner ADD MEMBER [NHSD];");
        }

        public static void DropUser(string config)
        {
            using var context = GetContext(config);

            context.Database.ExecuteSqlRaw("ALTER ROLE db_owner DROP MEMBER [NHSD];");
        }

        private static ApplicationDbContext GetContext(string config)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(config);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
