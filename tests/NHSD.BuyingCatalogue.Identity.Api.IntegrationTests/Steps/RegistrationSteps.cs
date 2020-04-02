using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class RegistrationSteps
    {
        private readonly string discovery;
        private readonly SeleniumContext _seleniumContext;

        public RegistrationSteps(IConfiguration configuration, SeleniumContext seleniumContext)
        {
            discovery = configuration.GetValue<string>("DiscoveryAddress");
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to identity url (.*)")]
        public void WhenTheUserNavigatesToUrl(string url)
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl($"{discovery}/{url}");
        }
    }
}
