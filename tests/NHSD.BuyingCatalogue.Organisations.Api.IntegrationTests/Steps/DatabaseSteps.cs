using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class DatabaseSteps
    {
        private readonly Config config;

        public DatabaseSteps(Config config)
        {
            this.config = config;
        }

        [Given(@"the call to the database will fail")]
        public async Task GivenTheCallToTheDatabaseWillFail()
        {
            await IntegrationDatabase.RemoveReadRoleAsync(config.AdminConnectionString);
            await IntegrationDatabase.RemoveWriteRoleAsync(config.AdminConnectionString);
        }

        [Given(@"The Database Server is down")]
        public async Task GivenTheDatabaseServerIsDown()
        {
            await IntegrationDatabase.DenyAccessForNhsdUser(config.AdminConnectionString);
        }
    }
}
