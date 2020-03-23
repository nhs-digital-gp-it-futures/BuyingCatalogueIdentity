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
        private readonly SeleniumContext _seleniumContext;

        public UiValidationSteps(SeleniumContext seleniumContext)
        {
            _seleniumContext = seleniumContext;
        }

        [Then(@"the page contains element with Data ID ([^\s]+)")]
        public void ThenThePageContainsElementWithDataId(string id)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
        }

        [Then(@"element with Data ID ([^\s]+) contains element with Data ID ([^\s]+)")]
        public void ThenTheElementContainsElementWithDataId(string parentElementDataId, string subElementDataId)
        {
            var element = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={parentElementDataId}]"));
            element.FindElement(By.CssSelector($"[data-test-id={subElementDataId}]"));
        }

        [Then(@"element with Data ID ([^\s]+) has text (.*)")]
        public void ThenElementHasText(string id, string text)
        {
            var element = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            element.Text.Should().Be(text);
        }

        [Then(@"element with Data ID ([^\s]+) has tag ([^\s]+)")]
        public void ThenTheElementHasTag(string id, string tag)
        {
            var element = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            element.TagName.Should().Be(tag);
        }

        [Then(@"element with Data ID ([^\s]+) is of type ([^\s]+)")]
        public void ThenElementIsOfType(string id, string type)
        {
            var element = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={id}]"));
            var typeAttribute = element.GetAttribute("type");
            typeAttribute.Should().Contain(type);
        }

        [Then(@"element with Data ID ([^\s]+) has label with text (.*)")]
        public void ThenTheElementHasLabelWithText(string dataId, string text)
        {
            var element = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var id = element.GetAttribute("id");
            var label = _seleniumContext.WebDriver.FindElement(By.CssSelector($"label[for={id}]"));
            label.Text.Should().Be(text);
        }

        [Then(@"element with Data ID ([^\s]+) contains a link to (.*)")]
        public void ThenTheElementIsALinkTo(string dataId, string link)
        {
            var element = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var linkElements = element.FindElements(By.TagName("a"));
            var linkElement = linkElements.FirstOrDefault(x => x.GetAttribute("href").EndsWith(link, StringComparison.OrdinalIgnoreCase));
            linkElement.Should().NotBeNull($"an element with link {link} should be found");
        }
    }
}
