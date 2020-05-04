using System;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LogoutSteps
    {
        private readonly SeleniumContext _context;
        private readonly string _identityApiBaseUrl;

        public LogoutSteps(SeleniumContext context, Settings settings)
        {
            _context = context;
            _identityApiBaseUrl = settings.IdentityApiBaseUrl;
        }

        [When(@"the user navigates directly to the logout page")]
        public void GivenTheUserNavigatesToTheLogoutPage()
        {
            _context.WebDriver.Navigate().GoToUrl($"{_identityApiBaseUrl}/Account/Logout");
        }
    }
}
