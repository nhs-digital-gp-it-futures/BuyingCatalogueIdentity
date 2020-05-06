using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UiInteractionSteps
    {
        private readonly SeleniumContext _seleniumContext;

        public UiInteractionSteps(SeleniumContext seleniumContext)
        {
            _seleniumContext = seleniumContext;
        }

        [When(@"element with Data ID (.*) is populated with (.*)")]
        public void WhenElementWithDataIdIsPopulatedWith(string dataId, string value)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"))
                .SendKeys(value);
        }

        [When(@"element with Data ID (.*) is clicked")]
        public void WhenElementWithDataIdIsClicked(string dataId)
        {
            _seleniumContext.WebDriver.FindElement(By.CssSelector($"[data-test-id={dataId}]"))
                .Click();
        }
    }
}
