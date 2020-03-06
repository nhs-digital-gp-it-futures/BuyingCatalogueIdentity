using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class OrganisationsSteps
    {
        private readonly ScenarioContext _context;
        private readonly IConfigurationRoot _config;
        private readonly string _organisationUrl = "http://docker.for.win.localhost:8075/api/v1/Organisations";

        public OrganisationsSteps(ScenarioContext context, IConfigurationRoot config)
        {
            _context = context;
            _config = config;
        }

        [Given(@"Organisations exist")]
        public async Task GivenOrganisationsExist(Table table)
        {
            foreach (var organisation in table.CreateSet<OrganisationTable>())
            {
                await InsertOrganisationsAsync(organisation);
            }
        }
     
        private async Task InsertOrganisationsAsync(OrganisationTable organisationTable)
        {
            var organisation = OrganisationEntityBuilder
                .Create()
                .WithName(organisationTable.Name)
                .WithOdsCode(organisationTable.OdsCode)
                .Build();

            await organisation.InsertAsync(_config.GetConnectionString("CatalogueUsers"));
        }

        [Given(@"the call to the database to set the field will fail")]
        public async Task GivenTheCallToTheDatabaseToSetTheFieldWillFail()
        {
            await Database.DropUser(_config.GetConnectionString("CatalogueUsersAdmin"));
        }
        
        [When(@"a GET request is made for the Organisations section")]
        public async Task WhenAGETRequestIsMadeForTheOrganisationsSection()
        {
            string bearerToken = _context["AccessToken"].ToString();
            
            using var client = new HttpClient();
           client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", bearerToken);

            //var sampleResource = "http://localhost:8071/Identity";

            var response = await client.GetAsync(new Uri(_organisationUrl));

            _context["Response"] = response;
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var organisations = table.CreateSet<OrganisationTable>().ToList();

            var response = _context["Response"] as HttpResponseMessage;
            response.Should().NotBeNull();

            var contentA = await response.Content.ReadAsStringAsync();

            var content = JToken.Parse(contentA);
            var organisationsContent = content.SelectToken("organisations").ToList();
            organisationsContent.Count().Should().Be(organisations.Count());

            organisationsContent[0].SelectToken("name").Value<string>().Should().Be(organisations[0].Name);
            organisationsContent[0].SelectToken("odsCode").Value<string>().Should().Be(organisations[0].OdsCode);

            organisationsContent[1].SelectToken("name").Value<string>().Should().Be(organisations[1].Name);
            organisationsContent[1].SelectToken("odsCode").Value<string>().Should().Be(organisations[1].OdsCode);

            organisationsContent[2].SelectToken("name").Value<string>().Should().Be(organisations[2].Name);
            organisationsContent[2].SelectToken("odsCode").Value<string>().Should().Be(organisations[2].OdsCode);
        }

        private class OrganisationTable
        {
            public string Name { get; set; }

            public string OdsCode { get; set; }
        }
    }
}
