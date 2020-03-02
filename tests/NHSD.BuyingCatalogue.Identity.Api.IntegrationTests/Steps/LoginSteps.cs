using System;
using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class LoginSteps
    {
        private readonly Dictionary<string, string> HostUrls = new Dictionary<string, string>
        {
            {"identity server", "http://host.docker.internal:8070/"},
            {"client", "http://host.docker.internal:8072/" }
        };

        private readonly ScenarioContext _context;
        private readonly SeleniumContext _seleniumContext;

        public LoginSteps(ScenarioContext context, SeleniumContext seleniumContext)
        {
            _context = context;
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to a restricted web page")]
        public void WhenTheUserNavigatesToTheLoginPage()
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl("http://host.docker.internal:8072/home/privacy");
        }

        [When(@"a login request is made with username (.*) and password (.*)")]
        public void WhenALoginRequestIsMade(string username, string password)
        {
            _seleniumContext.WebDriver.FindElement(By.Name("Username")).SendKeys(username);
            _seleniumContext.WebDriver.FindElement(By.Name("Password")).SendKeys(password);
            _seleniumContext.WebDriver.FindElement(By.TagName("form")).Submit();
        }

        [Then(@"the user is redirected to (identity server|client) page (.*)")]
        public void ThenTheUserIsRedirectedTo(string target, string url)
        {
            _seleniumContext.WebWaiter.Until(x => x.Url.StartsWith($"{HostUrls[target]}{url}",StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the page contains element with ID (.*) with text (.*)")]
        public void ThenThePageVerifiesItCouldTalkToTheSampleResource(string id, string text)
        {
            _seleniumContext.WebDriver.FindElement(By.Id(id)).Text.Should().Be(text);
        }
    }
}
