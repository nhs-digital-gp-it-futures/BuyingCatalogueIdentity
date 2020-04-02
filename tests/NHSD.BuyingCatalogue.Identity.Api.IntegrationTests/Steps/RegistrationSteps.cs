using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class RegistrationSteps
    {
        private IConfiguration _configuration { get; }
        private readonly SeleniumContext _seleniumContext;

        public RegistrationSteps(IConfiguration configuration, SeleniumContext seleniumContext)
        {
            _configuration = configuration;
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to identity url (.*)")]
        public void WhenTheUserNavigatesToUrl(string url)
        {
            var discoveryAddress = _configuration.GetValue<string>("DiscoveryAddress");

            _seleniumContext.WebDriver.Navigate().GoToUrl($"{discoveryAddress}/{url}");
        }
    }
}
