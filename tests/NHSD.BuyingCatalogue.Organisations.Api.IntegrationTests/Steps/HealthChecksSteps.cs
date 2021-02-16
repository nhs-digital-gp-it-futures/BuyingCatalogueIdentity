using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class HealthChecksSteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly ScenarioContext context;

        public HealthChecksSteps(Response response, Request request, ScenarioContext context, Config config)
        {
            this.response = response;
            this.request = request;
            this.context = context;
            this.context[ScenarioContextKeys.OrganisationsApiBaseUrl] = config.OrganisationsApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            await request.GetAsync(context.Get<Uri>(ScenarioContextKeys.OrganisationsApiBaseUrl), "health", "ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
