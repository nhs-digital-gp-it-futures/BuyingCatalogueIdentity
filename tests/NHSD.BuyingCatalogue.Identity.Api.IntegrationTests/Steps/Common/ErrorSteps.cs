using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
{
    [Binding]
    internal sealed class ErrorSteps
    {
        private readonly SeleniumContext _seleniumContext;

        public ErrorSteps(SeleniumContext seleniumContext)
        {
            _seleniumContext = seleniumContext;
        }

        [Then(@"the element with Data ID (.*) has validation error with text (.*)")]
        public void ThenThePageContainsValidationErrorWithText(string dataId, string value)
        {
            var emailGroup = _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"));
            var errorElement = emailGroup.FindElement(By.ClassName("field-validation-error"));
            errorElement.Text.Should().Be(value);
        }
    }
}
