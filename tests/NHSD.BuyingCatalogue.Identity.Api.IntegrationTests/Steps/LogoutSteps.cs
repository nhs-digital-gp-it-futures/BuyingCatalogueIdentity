using System;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LogoutSteps
    {
        private readonly SeleniumContext _context;
        private readonly string _identityApiBaseUrl;
        private readonly string _publicBrowseBaseUrl;

        public LogoutSteps(SeleniumContext context, Settings settings)
        {
            _context = context;
            _identityApiBaseUrl = settings.IdentityApiBaseUrl;
            _publicBrowseBaseUrl = settings.PublicBrowseBaseUrl;
        }

        [When(@"the user navigates directly to the logout page")]
        public void GivenTheUserNavigatesToTheLogoutPage()
        {
            _context.WebDriver.Navigate().GoToUrl($"{_identityApiBaseUrl}/Account/Logout");
        }

        [Then(@"the user is redirected to the public browse homepage")]
        public void ThenTheUserIsRedirectedToThePublicBrowseHomepage()
        {
            _context.WebWaiter.Until(x => string.Equals(x.Url, _publicBrowseBaseUrl, StringComparison.OrdinalIgnoreCase));
        }
    }
}
