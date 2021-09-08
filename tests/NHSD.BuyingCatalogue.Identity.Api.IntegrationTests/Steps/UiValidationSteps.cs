using System;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UiValidationSteps
    {
        private readonly SeleniumContext seleniumContext;

        public UiValidationSteps(SeleniumContext seleniumContext)
        {
            this.seleniumContext = seleniumContext;
        }

        [Then(@"the page contains element with Data ID ([^\s]+)")]
        public void ThenThePageContainsElementWithDataId(string id)
        {
            seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
        }

        [Then(@"element with Data ID ([^\s]+) contains element with Data ID ([^\s]+)")]
        public void ThenTheElementContainsElementWithDataId(string parentElementDataId, string subElementDataId)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={parentElementDataId}]"));
            element.FindElement(By.CssSelector($"[data-test-id={subElementDataId}]"));
        }

        [Then(@"element with Data ID ([^\s]+) has text (.*)")]
        public void ThenElementHasText(string id, string text)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            element.Text.Should().Be(text);
        }

        [Then(@"element with Data ID ([^\s]+) has an empty value")]
        public void ThenElementHasEmptyValue(string id)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            element.GetAttribute("value").Should().BeEmpty();
        }

        [Then(@"element with Data ID ([^\s]+) has tag ([^\s]+)")]
        public void ThenTheElementHasTag(string id, string tag)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            element.TagName.Should().Be(tag);
        }

        [Then(@"element with Data ID ([^\s]+) is of type ([^\s]+)")]
        public void ThenElementIsOfType(string id, string type)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            var typeAttribute = element.GetAttribute("type");
            typeAttribute.Should().Contain(type);
        }

        [Then(@"element with Data ID ([^\s]+) has label with text (.*)")]
        public void ThenTheElementHasLabelWithText(string dataId, string text)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var id = element.GetAttribute("id");
            var label = seleniumContext.WebDriver.FindElement(By.CssSelector($"label[for={id}]"));
            label.Text.Should().Be(text);
        }

        [Then(@"element with Data ID ([^\s]+) contains a link to ([^\s]+)")]
        public void ThenTheElementContainsALinkTo(string dataId, string link)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var linkElements = element.FindElements(By.TagName("a"));
            var linkElement = linkElements.FirstOrDefault(e => e.GetAttribute("href").EndsWith(link, StringComparison.OrdinalIgnoreCase));
            linkElement.Should().NotBeNull($"an element with link {link} should be found");
        }

        [Then(@"element with Data ID ([^\s]+) contains a link to ([^\s]+) with text (.*)")]
        public void ThenTheElementContainsALinkToWithText(string dataId, string link, string text)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            element.TagName.Should().Be("a");
            var linkElement = element.GetAttribute("href");
            linkElement.Should().NotBeNull($"an element with link {link} should be found");
            element.Text.Should().Be(text);
        }

        [When(@"the user clicks element with Data ID ([^\s]+)")]
        public void GivenTheUserClicksElementWithDataId(string dataId)
        {
            seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]")).Click();
        }

        [Then(@"element with Data ID ([^\s]+) is a link to (.*)")]
        public void ThenTheElementIsALinkTo(string dataId, string link)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var attribute = element.GetAttribute("href").Split("?")[0];
            attribute.Should().EndWithEquivalentOf(link);
        }

        [Then(@"element with Data ID ([^\s]+) is email link to address (.*)")]
        public void ThenElementWithDataIdIsEmailLinkToAddress(string dataId, string emailAddress)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var attribute = element.GetAttribute("href");
            attribute.Should().StartWithEquivalentOf($"mailto:{emailAddress}");
        }

        [Then(@"element with Data ID ([^\s]+) has the accessibility text (.*)")]
        public void ThenElementHasTheAccessibilityText(string id, string text)
        {
            var element = seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            element.GetAttribute("aria-label").Should().Be(text);
        }
    }
}
