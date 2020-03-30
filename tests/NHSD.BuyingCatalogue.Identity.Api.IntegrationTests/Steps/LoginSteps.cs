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

        [When(@"element with Data ID (.*) is populated with (.*)")]
        public void WhenElementWithDataIdIsPopulatedWith(string dataId, string value)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"))
                .SendKeys(value);
        }

        [When(@"element with Data ID (.*) is clicked")]
        public void WhenElementWithDataIdIsClicked(string dataId)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"))
                .Click();
        }

        [When(@"the form with Data ID (.*) is submitted")]
        public void WhenTheFormWithDataIdIsSubmitted(string dataId)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]")).Submit();
        }

        [When(@"a login request is made with email address (.*) and password (.*)")]
        public void WhenALoginRequestIsMade(string emailAddress, string password)
        {
            _seleniumContext.WebDriver.FindElement(By.Name("EmailAddress")).SendKeys(emailAddress);
            _seleniumContext.WebDriver.FindElement(By.Name("Password")).SendKeys(password);
            _seleniumContext.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [When(@"a login request is made with email address (.*) and no password")]
        public void WhenALoginRequestIsMadeWithNoPassword(string emailAddress)
        {
            _seleniumContext.WebDriver.FindElement(By.Name("EmailAddress")).SendKeys(emailAddress);
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

        [Then(@"the page contains a validation summary with text (.*)")]
        public void ThenThePageContainsValidationSummaryWithText(string value)
        {
            var errorElements = _seleniumContext.WebDriver.FindElements(By.CssSelector(".validation-summary-errors li"));
            errorElements.Should().HaveCount(1);
            errorElements.First().Text.Should().Be(value);
        }

        [Then(@"the page contains an email address error with text (.*)")]
        public void ThenThePageContainsEmailAddressErrorWithText(string value)
        {
            var emailGroup = _seleniumContext.WebDriver.FindElement(By.CssSelector("[data-test-id=email-field]"));
            var errorElement = emailGroup.FindElement(By.ClassName("field-validation-error"));
            errorElement.Text.Should().Be(value);
        }

        [Then(@"the page contains a password error with text (.*)")]
        public void ThenThePageContainsPasswordErrorWithText(string value)
        {
            var passwordGroup = _seleniumContext.WebDriver.FindElement(By.CssSelector("[data-test-id=password-field]"));
            var errorElement = passwordGroup.FindElement(By.ClassName("field-validation-error"));
            errorElement.Text.Should().Be(value);
        }

        [Given(@"a user has successfully logged in with email address (.*) and password (.*)")]
        public void GivenAUserHasLoggedIn(string email, string password)
        {
            WhenTheUserNavigatesToARestrictedPage();
            ThenTheUserIsRedirectedTo("account/login");
            WhenALoginRequestIsMade(email, password);
            ThenTheUserIsRedirectedTo("home/privacy");
            ThenThePageVerifiesItCouldTalkToTheSampleResource("sampleResourceResult", "Authorized With Sample Resource");
        }

        [When(@"the user clicks on the forgot password button")]
        public void WhenUserClicksOnForgotPassword()
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector("[data-test-id=forgot-password-link]")).Click();
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
