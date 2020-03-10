using System.Net.Http;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly ScenarioContext _context;

        public CommonSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Then(@"a response with status code ([\d]+) is returned")]
        public void AResponseIsReturned(int code)
        {
            var response = _context["Response"] as HttpResponseMessage;
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(code);
        }
    }
}
