using System;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LoginSteps
    {
        private readonly SeleniumContext _seleniumContext;

        private const string MvcBaseUrl = "http://host.docker.internal:8072";

        public LoginSteps(SeleniumContext seleniumContext)
        {
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to a restricted web page")]
        public void WhenTheUserNavigatesToARestrictedPage()
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl($"{MvcBaseUrl}/home/privacy");
        }

        [When(@"the redirect URL is modified to be invalid")]
        public void WhenTheRedirectURLIsModifiedToBeInvalid()
        {
            var currentUrl = _seleniumContext.WebDriver.Url;
            _seleniumContext.WebDriver.Url =
                currentUrl.Replace("signin-oidc", "invalid", StringComparison.OrdinalIgnoreCase);
        }

        [When(@"a login request is made with email address (.*) and password (.*)")]
        public void WhenALoginRequestIsMade(string emailAddress, string password)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector("[data-test-id=input-email-address]")).SendKeys(emailAddress);
            _seleniumContext.WebDriver.FindElement(By.CssSelector("[data-test-id=input-password]")).SendKeys(password);
            _seleniumContext.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [When(@"a login request is made with email address (.*) and no password")]
        public void WhenALoginRequestIsMadeWithNoPassword(string emailAddress)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector("[data-test-id=input-email-address]")).SendKeys(emailAddress);
            _seleniumContext.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [Then(@"the user is redirected to page (.*)")]
        public void ThenTheUserIsRedirectedTo(string url)
        {
            _seleniumContext.WebWaiter.Until(x => new Uri(x.Url).AbsolutePath.Contains(url, StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the page contains element with ID ([^\s]+) with text (.*)")]
        public void ThenThePageVerifiesItCouldTalkToTheSampleResource(string id, string text)
        {
            _seleniumContext.WebDriver.FindElement(By.Id(id)).Text.Should().Be(text);
        }

        [Then(@"the page contains a validation summary with text (.*) at position (.*)")]
        public void ThenThePageContainsValidationSummaryWithText(string value, int position)
        {
            var errorElements = _seleniumContext.WebDriver.FindElements(By.CssSelector("[data-test-id=error-summary] li"));
            errorElements[position].Text.Should().Be(value);
        }

        [Given(@"a user has successfully logged in with email address (.*) and password (.*)")]
        public void GivenAUserHasLoggedIn(string email, string password)
        {
            WhenTheUserNavigatesToARestrictedPage();
            ThenTheUserIsRedirectedTo("identity/account/login");
            WhenALoginRequestIsMade(email, password);
            ThenTheUserIsRedirectedTo("home/privacy");
            ThenThePageVerifiesItCouldTalkToTheSampleResource("sampleResourceResult", "Authorized With Sample Resource");
        }

        [When(@"the user clicks on logout button")]
        public void WhenUserClicksOnLogout()
        {
            _seleniumContext.WebDriver.FindElement(By.Id("logout")).Click();
        }

        [Then(@"the user is logged out")]
        public void ThenUserIsLoggedOut()
        {
            _seleniumContext.WebDriver.Url.Should().BeEquivalentTo($"{MvcBaseUrl}/");
        }
    }
}
