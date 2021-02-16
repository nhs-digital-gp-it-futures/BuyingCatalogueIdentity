﻿using System;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class RegistrationSteps
    {
        private readonly ScenarioContext _context;
        private readonly Uri _discovery;
        private readonly SeleniumContext _seleniumContext;

        public RegistrationSteps(
            IConfiguration configuration,
            ScenarioContext context,
            SeleniumContext seleniumContext)
        {
            _context = context;
            _discovery = configuration.GetValue<Uri>("DiscoveryAddress");
            _seleniumContext = seleniumContext;
        }

        [When(@"the user navigates to identity url (\S*)$")]
        public void WhenTheUserNavigatesToUrl(string url)
        {
            _seleniumContext.WebDriver.Navigate().GoToUrl(new Uri(_discovery, url));
        }

        [When(@"the user navigates to identity url (\S*) with a password reset token")]
        public void WhenTheUserNavigatesToUrlWithValidPasswordResetToken(string url)
        {
            var user = _context.Get<IdentityUser>();
            var encodedToken = HttpUtility.UrlEncode(_context.Get<string>(ScenarioContextKeys.PasswordResetToken));

            _seleniumContext.WebDriver.Navigate().GoToUrl(new Uri(_discovery, $"{url}?email={user.Email}&token={encodedToken}"));
        }
    }
}
