using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class HealthChecksSteps
    {
        private readonly Response _response;
        private readonly ScenarioContext _context;
        private readonly Settings _settings;

        public HealthChecksSteps(Response response, ScenarioContext context, Settings settings)
        {
            _response = response;
            _context = context;
            _settings = settings;
            _context["organisationBaseUrl"] = _settings.OrganisationApiBaseUrl;
        }

        [Given(@"The (?:Smtp|Database) Server is (up|down)")]
        public void GivenTheServerIsInState(string state)
        {
            _context["organisationBaseUrl"] = state == "up" ? _settings.OrganisationApiBaseUrl : _settings.BrokenOrganisationApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            using var client = new HttpClient();
            _response.Result = await client.GetAsync($"{_context["organisationBaseUrl"]}/health/ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await _response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
