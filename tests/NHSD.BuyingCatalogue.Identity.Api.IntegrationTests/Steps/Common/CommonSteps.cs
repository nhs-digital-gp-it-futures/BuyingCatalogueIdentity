using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

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

        [Then(@"the response contains the following errors")]
        public async Task ThenTheResponseContainsTheFollowingErrors(Table table)
        {
            var expected = table.CreateSet<UserErrorsTable>();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = response
                .SelectToken("errors")
                .Select(t => new UserErrorsTable
                {
                    ErrorMessageId = t.Value<string>("id"),
                    FieldName = t.Value<string>("field")
                });

            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class UserErrorsTable
        {
            public string ErrorMessageId { get; set; }

            public string FieldName { get; set; }
        }
    }
}
