using System;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LogoutSteps
    {
        private readonly SeleniumContext _context;
        private readonly Uri _identityApiBaseUrl;

        public LogoutSteps(SeleniumContext context, Settings settings)
        {
            _context = context;
            _identityApiBaseUrl = settings.IdentityApiBaseUrl;
        }

        [When(@"the user navigates directly to the logout page")]
        public void GivenTheUserNavigatesToTheLogoutPage()
        {
            _context.WebDriver.Navigate().GoToUrl(new Uri(_identityApiBaseUrl, "Account/Logout"));
        }
    }
}
