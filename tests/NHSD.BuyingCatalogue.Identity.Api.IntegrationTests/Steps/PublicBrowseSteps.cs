using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class PublicBrowseSteps
    {
        private readonly SeleniumContext context;
        private readonly Uri publicBrowseBaseUrl;
        private readonly Uri publicBrowseLoginUrl;
        private readonly Uri publicBrowseLogoutUrl;

        public PublicBrowseSteps(SeleniumContext context, Settings settings)
        {
            this.context = context;
            publicBrowseBaseUrl = settings.PublicBrowseBaseUrl;
            publicBrowseLoginUrl = settings.PublicBrowseLoginUrl;
            publicBrowseLogoutUrl = settings.PublicBrowseLogoutUrl;
        }

        [Then(@"the user is redirected to the public browse homepage")]
        public void ThenTheUserIsRedirectedToThePublicBrowseHomepage()
        {
            context.WebWaiter.Until(w => string.Equals(
                w.Url,
                publicBrowseBaseUrl.ToString(),
                StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the user is redirected to the public browse login page")]
        public void ThenTheUserIsRedirectedToThePublicBrowseLoginPage()
        {
            context.WebWaiter.Until(w => string.Equals(
                w.Url,
                publicBrowseLoginUrl.ToString(),
                StringComparison.OrdinalIgnoreCase));
        }

        [Then(@"the user is redirected to the public browse logout page")]
        public void ThenTheUserIsRedirectedToThePublicBrowseLogoutPage()
        {
            context.WebWaiter.Until(w => string.Equals(
                w.Url,
                publicBrowseLogoutUrl.ToString(),
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
            var link = context.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataTestId}]"));
            var linkHref = link.GetAttribute("href");

            linkHref.Should().Be(publicBrowseBaseUrl.ToString());
        }
    }
}
