using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrganisationServiceRecipientSteps
    {
        private readonly ScenarioContext context;
        private readonly Response response;
        private readonly Request request;

        private readonly Uri organisationsUrl;

        public OrganisationServiceRecipientSteps(
            ScenarioContext context,
            Response response,
            Request request,
            Config config)
        {
            this.context = context;
            this.response = response;
            this.request = request;
            organisationsUrl = new Uri(config.OrganisationsApiBaseUrl, "api/v1/Organisations/");
        }

        [When(@"the user makes a request to retrieve the service recipients with an organisation name (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheServiceRecipientsWithAnOrganisationName(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);

            await request.GetAsync(new Uri(organisationsUrl, $"{organisationId}/service-recipients"));
        }

        [Then(@"The organisation service recipient is returned with the following values")]
        public async Task ThenTheOrganisationServiceRecipientIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedServiceRecipients = table.CreateSet<ServiceRecipientsTable>().ToList();

            var serviceRecipients = (await response.ReadBodyAsJsonAsync()).Select(CreateServiceRecipients);

            expectedServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        }

        private static ServiceRecipientsTable CreateServiceRecipients(JToken token)
        {
            return new()
            {
                Name = token.SelectToken("name")?.ToString(),
                OdsCode = token.SelectToken("odsCode")?.ToString(),
            };
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ServiceRecipientsTable
        {
            public string Name { get; init; }

            public string OdsCode { get; init; }
        }
    }
}
