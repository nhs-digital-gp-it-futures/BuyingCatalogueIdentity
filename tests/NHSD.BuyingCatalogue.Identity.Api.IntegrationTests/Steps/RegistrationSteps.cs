using System.Web;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class RegistrationSteps
    {
        private readonly ScenarioContext _context;
        private readonly string _discovery;
        private readonly SeleniumContext _seleniumContext;

        public RegistrationSteps(
            IConfiguration configuration,
            ScenarioContext context,
            SeleniumContext seleniumContext)
        {
            _context = context;
            _discovery = configuration.GetValue<string>("DiscoveryAddress");
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to identity url (.*)")]
        public void WhenTheUserNavigatesToUrl(string url)
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl($"{_discovery}/{url}");
        }

        [When(@"the user with e-mail address (.*) navigates to identity url (.*) with a valid password reset token")]
        public void WhenTheUserNavigatesToUrlWithValidPasswordResetToken(string emailAddress, string url)
        {
            var token = HttpUtility.UrlEncode((string)_context[ScenarioContextKeys.PasswordResetToken]);
            _seleniumContext.WebDriver.Navigate().GoToUrl($"{_discovery}/{url}?email={emailAddress}&token={token}");
        }
    }
}
