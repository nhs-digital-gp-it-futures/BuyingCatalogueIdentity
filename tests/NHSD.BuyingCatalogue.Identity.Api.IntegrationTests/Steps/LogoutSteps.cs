using System;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LogoutSteps
    {
        private readonly SeleniumContext context;
        private readonly Uri identityApiBaseUrl;

        public LogoutSteps(SeleniumContext context, Settings settings)
        {
            this.context = context;
            identityApiBaseUrl = settings.IdentityApiBaseUrl;
        }

        [When(@"the user navigates directly to the logout page")]
        public void GivenTheUserNavigatesToTheLogoutPage()
        {
            context.WebDriver.Navigate().GoToUrl(new Uri(identityApiBaseUrl, "Account/Logout"));
        }
    }
}
