using System;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
{
    [Binding]
    internal sealed class StepArgumentTransformations
    {
        [StepArgumentTransformation]
        internal static string ParseNewLineString(string value) => value.Replace(@"\n", Environment.NewLine);
    }
}
