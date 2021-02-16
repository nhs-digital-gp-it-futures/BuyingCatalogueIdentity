﻿using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class PublicBrowseSteps
    {
        private readonly SeleniumContext _context;
        private readonly Uri _publicBrowseBaseUrl;
        private readonly Uri _publicBrowseLoginUrl;
        private readonly Uri _publicBrowseLogoutUrl;

        public PublicBrowseSteps(SeleniumContext context, Settings settings)
        {
            _context = context;
            _publicBrowseBaseUrl = settings.PublicBrowseBaseUrl;
            _publicBrowseLoginUrl = settings.PublicBrowseLoginUrl;
            _publicBrowseLogoutUrl = settings.PublicBrowseLogoutUrl;
        }

        [Then(@"the user is redirected to the public browse homepage")]
        public void ThenTheUserIsRedirectedToThePublicBrowseHomepage()
        {
            _context.WebWaiter.Until(w => string.Equals(
                w.Url,
                _publicBrowseBaseUrl.ToString(),
                StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the user is redirected to the public browse login page")]
        public void ThenTheUserIsRedirectedToThePublicBrowseLoginPage()
        {
            _context.WebWaiter.Until(w => string.Equals(
                w.Url,
                _publicBrowseLoginUrl.ToString(),
                StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the user is redirected to the public browse logout page")]
        public void ThenTheUserIsRedirectedToThePublicBrowseLogoutPage()
        {
            _context.WebWaiter.Until(w => string.Equals(
                w.Url,
                _publicBrowseLogoutUrl.ToString(),
                StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the NHS header logo should link to the public browse homepage")]
        public void ThenTheNhsHeaderLogoShouldLinkToThePublicBrowseHomepage()
        {
            AssertHrefAttributeIsEqualToPublicBrowseHomepage("header-logo");
        }

        [Then(@"the go back element should link to the public browse homepage")]
        public void ThenTheGoBackElementShouldLinkToThePublicBrowseHomepage()
        {
            AssertHrefAttributeIsEqualToPublicBrowseHomepage("go-back-link");
        }

        private void AssertHrefAttributeIsEqualToPublicBrowseHomepage(string dataTestId)
        {
            var link = _context.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataTestId}]"));
            var linkHref = link.GetAttribute("href");

            linkHref.Should().Be(_publicBrowseBaseUrl.ToString());
        }
    }
}
