using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps.Common
{
    [Binding]
    internal sealed class CommonSteps
    {
        private readonly Response response;

        public CommonSteps(Response response)
        {
            this.response = response;
        }

        [Then(@"a response with status code ([\d]+) is returned")]
        public void AResponseIsReturned(int code)
        {
            response.Should().NotBeNull();
            response.Result.StatusCode.Should().Be(code);
        }

        [Then(@"the response contains the following errors")]
        public async Task ThenTheResponseContainsTheFollowingErrors(Table table)
        {
            var expected = table.CreateSet<ResponseErrorsTable>();

            var jsonBody = await response.ReadBodyAsJsonAsync();

            var actual = jsonBody
                .SelectToken("errors")?
                .Select(t => new ResponseErrorsTable
                {
                    ErrorMessageId = t.Value<string>("id"),
                    FieldName = t.Value<string>("field"),
                });

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"a response with an empty body is returned")]
        public async Task ThenTheResponseBodyShouldBeEmpty()
        {
            var jsonbody = await response.ReadBodyAsJsonAsync();

            jsonbody.Should().BeEmpty();
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ResponseErrorsTable
        {
            public string ErrorMessageId { get; init; }

            public string FieldName { get; init; }
        }
    }
}
