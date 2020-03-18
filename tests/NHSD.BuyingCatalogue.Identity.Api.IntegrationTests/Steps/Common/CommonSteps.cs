using FluentAssertions;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly Response _response;

        public CommonSteps(Response response)
        {
            _response = response;
        }

        [Then(@"a response with status code ([\d]+) is returned")]
        public void AResponseIsReturned(int code)
        {
            _response.Should().NotBeNull();
            _response.Result.StatusCode.Should().Be(code);
        }
    }
}
