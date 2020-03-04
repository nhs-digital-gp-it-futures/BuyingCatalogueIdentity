using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    [Binding]
    public sealed class Database
    {
        [BeforeScenario]
        public void Reset(IConfigurationRoot config)
        {
            using var context = GetContext(config.GetConnectionString("CatalogueUsersAdmin"));

            context.Database.ExecuteSqlRaw("ALTER ROLE db_owner ADD MEMBER [NHSD];");

            context.Organisations.RemoveRange(context.Organisations);

            context.SaveChanges();
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
