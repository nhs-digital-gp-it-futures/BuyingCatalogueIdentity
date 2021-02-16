using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LoginSteps
    {
        private readonly SeleniumContext context;

        private readonly Uri sampleMvcBaseUrl;
        private readonly Uri identityApiBaseUrl;

        public LoginSteps(SeleniumContext context, Settings settings)
        {
            this.context = context;

            sampleMvcBaseUrl = settings.SampleMvcClientBaseUrl;
            identityApiBaseUrl = settings.IdentityApiBaseUrl;
        }

        [When(@"the user navigates to a restricted web page")]
        public void WhenTheUserNavigatesToARestrictedPage()
        {
            context.WebDriver.Navigate().GoToUrl(new Uri(sampleMvcBaseUrl, "/home/privacy"));
        }

        [When(@"the redirect URL is modified to be invalid")]
        public void WhenTheRedirectUrlIsModifiedToBeInvalid()
        {
            var currentUrl = context.WebDriver.Url;

            context.WebDriver.Url = currentUrl.Replace("signin-oidc", "invalid", StringComparison.OrdinalIgnoreCase);
        }

        [When(@"a login request is made with email address (.*) and password (.*)")]
        public void WhenALoginRequestIsMade(string emailAddress, string password)
        {
            context.WebDriver.FindElement(By.CssSelector("[data-test-id=input-email-address]")).SendKeys(emailAddress);
            context.WebDriver.FindElement(By.CssSelector("[data-test-id=input-password]")).SendKeys(password);
            context.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [When(@"a login request is made with email address (.*) and no password")]
        public void WhenALoginRequestIsMadeWithNoPassword(string emailAddress)
        {
            context.WebDriver.FindElement(By.CssSelector("[data-test-id=input-email-address]")).SendKeys(emailAddress);
            context.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [Then(@"the user is redirected to page (.*)")]
        public void ThenTheUserIsRedirectedTo(string url)
        {
            context.WebWaiter.Until(d => new Uri(d.Url).AbsolutePath.Contains(url, StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the user is redirected to the Url (.*)")]
        public void ThenTheUserIsRedirectedToTheUrl(string url)
        {
            context.WebWaiter.Until(d => string.Equals(d.Url, url, StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the page contains element with ID ([^\s]+) with text (.*)")]
        public void ThenThePageVerifiesItCouldTalkToTheSampleResource(string id, string text)
        {
            context.WebDriver.FindElement(By.Id(id)).Text.Should().Be(text);
        }

        [Then(@"the page contains a validation summary with text (.*) at position (.*)")]
        public void ThenThePageContainsValidationSummaryWithText(string value, int position)
        {
            var errorElements = context.WebDriver.FindElements(By.CssSelector("[data-test-id=error-summary] li"));
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
            context.WebDriver.FindElement(By.Id("logout")).Click();
        }

        [Then(@"the user is logged out")]
        public void ThenUserIsLoggedOut()
        {
            context.WebDriver.Url.Should().BeEquivalentTo($"{sampleMvcBaseUrl}");
        }

        [Given(@"the user navigates directly to the login page")]
        public void GivenTheUserNavigatesToTheLoginPage()
        {
            context.WebDriver.Navigate().GoToUrl(new Uri(identityApiBaseUrl, "Account/Login"));
        }
    }
}
