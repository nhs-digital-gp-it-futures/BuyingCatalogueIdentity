using System;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class RegistrationSteps
    {
        private readonly ScenarioContext context;
        private readonly Uri discovery;
        private readonly SeleniumContext seleniumContext;

        public RegistrationSteps(
            IConfiguration configuration,
            ScenarioContext context,
            SeleniumContext seleniumContext)
        {
            this.context = context;
            discovery = configuration.GetValue<Uri>("DiscoveryAddress");
            this.seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to identity url (\S*)$")]
        public void WhenTheUserNavigatesToUrl(string url)
        {
            seleniumContext.WebDriver.Navigate().GoToUrl(new Uri(discovery, url));
        }

        [When(@"the user navigates to identity url (\S*) with a password reset token")]
        public void WhenTheUserNavigatesToUrlWithValidPasswordResetToken(string url)
        {
            var user = context.Get<IdentityUser>();
            var encodedToken = HttpUtility.UrlEncode(context.Get<string>(ScenarioContextKeys.PasswordResetToken));

            seleniumContext.WebDriver.Navigate().GoToUrl(new Uri(discovery, $"{url}?email={user.Email}&token={encodedToken}"));
        }
    }
}
