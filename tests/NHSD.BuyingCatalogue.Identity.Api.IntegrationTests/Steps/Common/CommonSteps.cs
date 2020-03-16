using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
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
            _response.Result.StatusCode.Should().Be(code);
        }

        [Then(@"the string value of (.*) is (.*)")]
        public async Task ThenTheStringValueIs(string section, string value)
        {
            var content = await _response.ReadBody();

            var token = content.SelectToken(section) as JValue;
            token.Value.Should().Be(value);
        }

        [Then(@"the boolean value of (.*) is (.*)")]
        public async Task ThenTheBooleanValueIs(string section, bool value)
        {
            var content = await _response.ReadBody();
            content.Value<bool>(section).Should().Be(value);
        }
    }
}
