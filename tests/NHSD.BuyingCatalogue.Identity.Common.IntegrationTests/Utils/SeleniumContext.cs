﻿using System;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils
{
    public sealed class SeleniumContext : IDisposable
    {
        public IWebDriver WebDriver { get; }
        public WebDriverWait WebWaiter { get; }

        public SeleniumContext()
        {
            ChromeOptions options = new ChromeOptions { Proxy = null };
            options.AddArguments("window-size=1920,1080", "no-sandbox", "disable-dev-shm-usage");
            if (!Debugger.IsAttached)
            {
                options.AddArgument("headless");
                WebDriver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options);
            }
            else
            {
                WebDriver = new ChromeDriver(options);
            }
            WebWaiter = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            WebDriver?.Dispose();
        }
    }
}
