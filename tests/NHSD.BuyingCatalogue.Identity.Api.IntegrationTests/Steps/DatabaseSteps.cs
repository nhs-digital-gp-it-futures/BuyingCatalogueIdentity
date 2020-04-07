using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class DatabaseSteps
    {
        private readonly Settings _settings;
        private readonly ScenarioContext _context;

        public DatabaseSteps(Settings settings, ScenarioContext context)
        {
            _settings = settings;
            _context = context;
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
