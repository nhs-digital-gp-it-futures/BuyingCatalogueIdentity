using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class DatabaseSteps
    {
        private readonly Settings _settings;

        public DatabaseSteps(Settings settings)
        {
            _settings = settings;
        }

        [Given(@"the call to the database will fail")]
        public async Task GivenTheCallToTheDatabaseWillFail()
        {
            await IntegrationDatabase.RemoveReadRoleAsync(_settings.AdminConnectionString);
            await IntegrationDatabase.RemoveWriteRoleAsync(_settings.AdminConnectionString);
        }

        [Given(@"The Database Server is down")]
        public async Task GivenTheDatabaseServerIsDown()
        {
            await IntegrationDatabase.DenyAccessForNhsdUser(_settings.AdminConnectionString);
        }
    }
}
