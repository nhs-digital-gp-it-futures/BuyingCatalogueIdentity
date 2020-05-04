using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class PublicBrowseSteps
    {
        private readonly SeleniumContext _context;
        private readonly string _publicBrowseBaseUrl;

        public PublicBrowseSteps(SeleniumContext context, Settings settings)
        {
            _context = context;
            _publicBrowseBaseUrl = settings.PublicBrowseBaseUrl;
        }

        [Then(@"the user is redirected to the public browse homepage")]
        public void ThenTheUserIsRedirectedToThePublicBrowseHomepage()
        {
            _context.WebWaiter.Until(x => string.Equals(x.Url, _publicBrowseBaseUrl, StringComparison.OrdinalIgnoreCase));
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

            linkHref.Should().Be(_publicBrowseBaseUrl);
        }
    }
}
