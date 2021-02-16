using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class HealthChecksSteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly ScenarioContext context;
        private readonly Settings settings;

        public HealthChecksSteps(Response response, Request request, ScenarioContext context, Settings settings)
        {
            this.response = response;
            this.request = request;
            this.context = context;
            this.settings = settings;
            this.context[ScenarioContextKeys.IdentityApiBaseUrl] = settings.IdentityApiBaseUrl;
        }

        [Given(@"The Smtp Server is (up|down)")]
        public void GivenTheIdentitySmtpServerIsInState(string state)
        {
            context[ScenarioContextKeys.IdentityApiBaseUrl] = state == "up"
                ? settings.IdentityApiBaseUrl
                : settings.BrokenSmtpIdentityApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            await request.GetAsync(context.Get<Uri>(ScenarioContextKeys.IdentityApiBaseUrl), "health", "ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
