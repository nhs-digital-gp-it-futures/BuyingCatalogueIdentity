using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
{
    [Binding]
    internal sealed class StepArgumentTransformations
    {
        private static string newLine = @"
";

        [StepArgumentTransformation]
        internal static string ParseNewLineString(string value) => value.Replace(@"\n", newLine);
    }
}
