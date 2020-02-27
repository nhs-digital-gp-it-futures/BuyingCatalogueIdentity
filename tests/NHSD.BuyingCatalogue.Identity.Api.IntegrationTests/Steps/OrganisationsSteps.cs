using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class OrganisationsSteps
    {
        private ScenarioContext _context;
        private readonly string _organisationUrl = "api/v1/Organisations";

        public OrganisationsSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"Organisations exist")]
        public void GivenOrganisationsExist(Table table)
        {
            foreach (var organisation in table.CreateSet<OrganisationTable>())
            {
                OrganisationEntityBuilder
                    .Create()
                    .WithName(organisation.Name)
                    .WithOdsCode(organisation.OdsCode)
                    .Insert();
            }
        }

        [When(@"a GET request is made for the Organisations section")]
        public async Task WhenAGETRequestIsMadeForTheOrganisationsSection()
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(new Uri(_organisationUrl));

            _context["Response"] = response;
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var organisations = table.CreateInstance<OrganisationTable>();

            var response = _context["Response"] as HttpRequestMessage;
            response.Should().NotBeNull();

            var content = JToken.Parse(await response.Content.ReadAsStringAsync());
            content.Select(t => t.Value<string>().Should().BeEquivalentTo(organisations.Name));
        }


        private class OrganisationTable
        {
            public string Name { get; set; }

            public string OdsCode { get; set; }
        }
    }
}
