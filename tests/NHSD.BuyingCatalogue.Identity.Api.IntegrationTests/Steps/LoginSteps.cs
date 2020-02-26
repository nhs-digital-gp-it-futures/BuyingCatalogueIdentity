using System;
using System.IO;
using System.Net.Http;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal class LoginSteps : IDisposable
    {
        private readonly ScenarioContext _context;
      
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LoginSteps(ScenarioContext context)
        {
            _context = context;
            ChromeOptions options = new ChromeOptions();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                options.AddArguments("--start-maximized");
            }
            else
            {
                options.AddArguments("--headless");
            }
            
            _driver = new ChromeDriver(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"the client is using valid client ID and valid secret")]
        public void GivenTheClientIsUsingValidClientIDAndValidSecret()
        {
            _context["ApiPort"] = "8072";
        }

        [Given(@"the client is using valid client ID and invalid secret")]
        public void GivenTheClientIsUsingValidClientIDAndInvalidSecret()
        {
            _context["ApiPort"] = "8073";
        }

        [When(@"the user navigates to a restricted web page")]
        public void WhenTheUserNavigatesToARestrictedWebPage()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();

            _driver.Navigate().GoToUrl($"http://localhost:{_context["ApiPort"]}/Home/Privacy");
        }

        [Then(@"the user is redirected to the login screen")]
        public void ThenTheUserIsRedirectedToTheLoginScreen()
        {
            _wait.Until(x => x.Title.Contains("Login"));

            _driver.FindElement(By.TagName("form"));
        }

        [When(@"a login request is made with username (.*) and password (.*)")]
        public void WhenALoginRequestIsMade(string username, string password)
        {
            _driver.FindElement(By.Name("Username")).SendKeys(username);
            _driver.FindElement(By.Name("Password")).SendKeys(password);
            _driver.FindElement(By.TagName("form")).Submit();
        }

        [Then(@"the user is redirected to the restricted web page")]
        public void ThenTheUserIsRedirectedToTheRestrictedWebPage()
        {
            _wait.Until(x => x.Title.Contains("Congratulations!"));
        }

        [Then(@"the response should not contain unauthorised")]
        public void ThenTheResponseShouldNotContainUnauthorised()
        {
            _driver.FindElement(By.Id("response")).Text.Should().NotBe(("Unauthorised"));
        }

        public void Dispose()
        {
            _driver?.Quit();
        }
    }
}
