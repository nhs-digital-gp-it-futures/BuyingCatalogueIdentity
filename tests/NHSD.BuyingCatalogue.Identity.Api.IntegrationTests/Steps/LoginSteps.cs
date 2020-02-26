using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal class LoginSteps : IDisposable
    {
        private readonly ScenarioContext _context;
      
        private readonly IEnumerable<string> _emailAddresses;
        private readonly IEnumerable<string> _passwords;

        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        private const string Invalid = "Invalid";

        public LoginSteps(ScenarioContext context)
        {
            _context = context;
            _emailAddresses = _context["EmailAddresses"] as IEnumerable<string>;
            _passwords = _context["Passwords"] as IEnumerable<string>;
            ChromeOptions options = new ChromeOptions();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                options.AddArguments("--start-maximized");
            }
            else
            {
                options.AddArguments("--headless");
            }
            
            driver = new ChromeDriver(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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

        [Given(@"the credentials for the client are valid")]
        public void GivenTheCredentialsForTheClientAreValid()
        {
            _context["ClientUsername"] = _emailAddresses.First();
            _context["ClientPassword"] = _passwords.First();
        }

        [Given(@"the credentials for the client have an invalid username")]
        public void GivenTheCredentialsForTheClientHaveInvalidUsername()
        {
            _context["ClientUsername"] = _emailAddresses.First() + Invalid;
            _context["ClientPassword"] = _emailAddresses.First();
        }

        [Given(@"the credentials for the client have an invalid password")]
        public void GivenTheCredentialsForTheClientHaveInvalidPassword()
        {
            _context["ClientUsername"] = _emailAddresses.First();
            _context["ClientPassword"] = _emailAddresses.First() + Invalid;
        }

        [When(@"the user navigates to a restricted web page")]
        public void WhenTheUserNavigatesToARestrictedWebPage()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();

            driver.Navigate().GoToUrl($"http://localhost:{_context["ApiPort"]}/Home/Privacy");
        }

        [Then(@"the user is redirected to the login screen")]
        public void ThenTheUserIsRedirectedToTheLoginScreen()
        {
            wait.Until(x => x.Title.Contains("Login"));

            driver.FindElement(By.TagName("form"));
        }

        [When(@"a login request is made")]
        public void WhenALoginRequestIsMade()
        {
            driver.FindElement(By.Name("Username")).SendKeys((string)_context["ClientUsername"]);
            driver.FindElement(By.Name("Password")).SendKeys(_context["ClientUsername"].ToString());
            driver.FindElement(By.TagName("form")).Submit();
        }

        [Then(@"the user is redirected to the restricted web page")]
        public void ThenTheUserIsRedirectedToTheRestrictedWebPage()
        {
            wait.Until(x => x.Title.Contains("Congratulations!"));
        }

        [Then(@"the response should not contain unauthorised")]
        public void ThenTheResponseShouldNotContainUnauthorised()
        {
            driver.FindElement(By.Id("response")).Text.Should().NotBe(("Unauthorised"));
        }

        public void Dispose()
        {
            driver?.Quit();
        }
    }
}
