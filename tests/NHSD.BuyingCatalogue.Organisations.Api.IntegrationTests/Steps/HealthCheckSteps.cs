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
        private readonly Response _response;
        private readonly Request _request;
        private readonly ScenarioContext _context;

        public HealthChecksSteps(Response response, Request request, ScenarioContext context, Settings settings)
        {
            _response = response;
            _request = request;
            _context = context;
            _context[ScenarioContextKeys.OrganisationsApiBaseUrl] = settings.OrganisationsApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            await _request.GetAsync(_context.Get<string>(ScenarioContextKeys.OrganisationsApiBaseUrl), "health", "ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await _response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
