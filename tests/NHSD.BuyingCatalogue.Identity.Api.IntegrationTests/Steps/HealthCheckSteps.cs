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
            _context["identityBaseUrl"] = _settings.IdentityApiBaseUrl;
        }

        [Given(@"The Smtp Server is (up|down) for ISAPI")]
        public void GivenTheIdentitySmtpServerIsInState(string state)
        {
            _context["identityBaseUrl"] = state == "up" ? _settings.IdentityApiBaseUrl : _settings.BrokenSmtpIdentityApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit for (ISAPI|OAPI)")]
        public async Task WhenTheHealthCheckEndpointIsHit(string service)
        {
            var baseUrl = service == "ISAPI" ? _context["identityBaseUrl"] : _context["organisationBaseUrl"];
            using var client = new HttpClient();
            _response.Result = await client.GetAsync($"{baseUrl}/health/ready");
        }

        [Then(@"the response will be (Healthy|Degraded|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await _response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
