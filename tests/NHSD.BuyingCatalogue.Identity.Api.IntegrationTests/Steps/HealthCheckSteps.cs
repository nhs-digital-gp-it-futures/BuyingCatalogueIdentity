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
        private readonly Response _response;
        private readonly Request _request;
        private readonly ScenarioContext _context;
        private readonly Settings _settings;

        public HealthChecksSteps(Response response, Request request, ScenarioContext context, Settings settings)
        {
            _response = response;
            _request = request;
            _context = context;
            _settings = settings;
            _context[ScenarioContextKeys.IdentityApiBaseUrl] = _settings.IdentityApiBaseUrl;
        }

        [Given(@"The Smtp Server is (up|down)")]
        public void GivenTheIdentitySmtpServerIsInState(string state)
        {
            _context[ScenarioContextKeys.IdentityApiBaseUrl] = state == "up" ? _settings.IdentityApiBaseUrl : _settings.BrokenSmtpIdentityApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            await _request.GetAsync(_context.Get<Uri>(ScenarioContextKeys.IdentityApiBaseUrl), "health", "ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await _response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
