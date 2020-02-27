using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public class SeleniumContext : IDisposable
    {
        public IWebDriver WebDriver { get; }
        public WebDriverWait WebWaiter{ get; }

        public SeleniumContext()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless", "window-size=1920,1080", "no-sandbox", "disable-dev-shm-usage");
            WebDriver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options);
            WebWaiter = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            WebDriver?.Dispose();
        }
    }
}
