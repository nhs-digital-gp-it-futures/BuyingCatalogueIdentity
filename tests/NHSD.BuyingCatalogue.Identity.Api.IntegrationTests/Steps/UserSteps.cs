using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UserSteps
    {
        private readonly ScenarioContext _context;

        public UserSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"There are Users in the database")]
        public void GivenThereAreUsersInTheDatabase(Table table)
        {
            var users = table.CreateSet<UserTable>();

            // TODO: Do something with these, either verify them in the DB or create them
            _context["EmailAddresses"] = users.Select(x => x.EmailAddress);
            _context["Passwords"] = users.Select(x => x.Password);
        }

        private class UserTable
        {
            public string EmailAddress { get; set; }

            public string Password { get; set; }
        }

    }
}
