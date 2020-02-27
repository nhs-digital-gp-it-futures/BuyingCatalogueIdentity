using System.Net.Http;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal class LoginSteps
    {
        private readonly ScenarioContext _context;
        private readonly SeleniumContext _seleniumContext;
        public LoginSteps(ScenarioContext context, SeleniumContext seleniumContext)
        {
            _context = context;
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to the login page with return url (.*)")]
        public void WhenTheUserNavigatesToTheLoginPage(string returnUrl)
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl($"http://host.docker.internal:8070/account/login?returnUrl={returnUrl}");
        }

        [When(@"a login request is made with username (.*) and password (.*)")]
        public void WhenALoginRequestIsMade(string username, string password)
        {
            _seleniumContext.WebDriver.FindElement(By.Name("Username")).SendKeys(username);
            _seleniumContext.WebDriver.FindElement(By.Name("Password")).SendKeys(password);
            _seleniumContext.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [Then(@"The user is redirected to (.*)")]
        public void ThenTheUserIsRedirectedTo(string url)
        {
            _seleniumContext.WebWaiter.Until(x => x.Url == url);
        }
    }
}
