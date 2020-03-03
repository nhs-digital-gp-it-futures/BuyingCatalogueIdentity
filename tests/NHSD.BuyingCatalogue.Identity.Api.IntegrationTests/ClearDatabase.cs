using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests
{
    [Binding]
    public sealed class ClearDatabase
    {
        [AfterScenario]
        public void Clear(IConfigurationRoot config)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("CatalogueUsersAdmin"));
            var context = new ApplicationDbContext(optionsBuilder.Options);

            context.Organisations.RemoveRange(context.Organisations);

            context.SaveChanges();
        }
    }
}
