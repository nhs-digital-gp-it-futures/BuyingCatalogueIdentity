using System.Web;
using Microsoft.AspNetCore.Identity;
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

        [When(@"the user navigates to identity url (\S*)$")]
        public void WhenTheUserNavigatesToUrl(string url)
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl($"{_discovery}/{url}");
        }

        [When(@"the user navigates to identity url (\S*) with a valid password reset token")]
        public void WhenTheUserNavigatesToUrlWithValidPasswordResetToken(string url)
        {
            var user = (IdentityUser)_context[ScenarioContextKeys.IdentityUser];
            var encodedToken = HttpUtility.UrlEncode((string)_context[ScenarioContextKeys.PasswordResetToken]);

            _seleniumContext.WebDriver.Navigate().GoToUrl($"{_discovery}/{url}?email={user.Email}&token={encodedToken}");
        }
    }
}
