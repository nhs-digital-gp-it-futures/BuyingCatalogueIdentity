using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
{
    internal static class ScenarioContextExtensions
    {
        public static TValue Get<TValue>(this ScenarioContext context, string key, TValue defaultValue) => 
            context.TryGetValue(key, out TValue value) ? value : defaultValue;
    }
}
