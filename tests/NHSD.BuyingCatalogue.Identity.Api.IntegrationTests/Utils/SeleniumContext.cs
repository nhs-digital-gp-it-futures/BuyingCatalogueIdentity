using System;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    [Binding]
    internal sealed class SeleniumContext : IDisposable
    {
        public SeleniumContext()
        {
            WebDriver = CreateWebDriver();
            WebWaiter = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
        }

        public IWebDriver WebDriver { get; }

        public WebDriverWait WebWaiter { get; }

        public void Dispose()
        {
            WebDriver?.Dispose();
        }

        private static IWebDriver CreateWebDriver()
        {
            var options = new ChromeOptions { Proxy = null };

            options.AddArguments("window-size=1920,1080", "no-sandbox", "disable-dev-shm-usage", "ignore-certificate-errors");

            if (Debugger.IsAttached)
                return new ChromeDriver(options);

            options.AddArgument("headless");
            return new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options);
        }
    }
}
