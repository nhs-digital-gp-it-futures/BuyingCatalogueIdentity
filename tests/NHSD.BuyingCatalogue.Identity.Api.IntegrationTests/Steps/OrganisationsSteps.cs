using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class OrganisationsSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly IConfigurationRoot _config;
        private readonly string _organisationUrl = "http://localhost:8075/api/v1/Organisations";

        private readonly string _configAccessUserAdmin = "CatalogueUsersAdmin";
        private readonly string _configAccessUser = "CatalogueUsers";

        private readonly string _accessToken = "AccessToken";
        private readonly string _listOrganisations = "ListOrganisations";

        public OrganisationsSteps(ScenarioContext context, Response response, IConfigurationRoot config)
        {
            _context = context;
            _response = response;
            _config = config;
        }

        [Given(@"Organisations exist")]
        public async Task GivenOrganisationsExist(Table table)
        {
            foreach (var organisation in table.CreateSet<OrganisationTable>())
            {
                await InsertOrganisationsAsync(organisation);
            }

            _context[_listOrganisations] =
                await OrganisationEntity.FetchAllOrganisationsAsync(_config.GetConnectionString(_configAccessUser));
        }

        private async Task InsertOrganisationsAsync(OrganisationTable organisationTable)
        {
            var organisation = OrganisationEntityBuilder
                .Create()
                .WithName(organisationTable.Name)
                .WithOdsCode(organisationTable.OdsCode)
                .Build();

            await organisation.InsertAsync(_config.GetConnectionString(_configAccessUser));
        }

        [Given(@"the call to the database to set the field will fail")]
        public async Task GivenTheCallToTheDatabaseToSetTheFieldWillFail()
        {
            await Database.DropUser(_config.GetConnectionString(_configAccessUserAdmin));
        }

        [When(@"a GET request is made for the Organisations section")]
        public async Task WhenAGETRequestIsMadeForTheOrganisationsSection()
        {
            CheckAccessToken();

            string bearerToken = _context[_accessToken].ToString();

            using var client = new HttpClient();
            client.SetBearerToken(bearerToken);

            _response.Result = await client.GetAsync(new Uri(_organisationUrl));
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisations = table.CreateSet<OrganisationTable>().ToList();

            var organisations = (await _response.ReadBody()).SelectToken("organisations").Select(t => new
            {
                Name = t.SelectToken("name").ToString(), OdsCode = t.SelectToken("odsCode").ToString()
            });

            organisations.Should().BeEquivalentTo(expectedOrganisations, options => options.WithStrictOrdering());
        }

        [When(@"a GET request is made for an organisation with name (.*)")]
        public async Task WhenAGETRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            CheckAccessToken();

            string bearerToken = _context[_accessToken].ToString();

            using var client = new HttpClient();
            client.SetBearerToken(bearerToken);

            _context.ContainsKey(_listOrganisations).Should().BeTrue();
            var allOrganisations = _context[_listOrganisations] as IEnumerable<OrganisationEntity>;

            var organisation = allOrganisations.FirstOrDefault(x => x.Name == organisationName);
            var orgId = Guid.Empty;
            if (organisation != null)
            {
                orgId = organisation.Id;
            }

            _response.Result = await client.GetAsync(new Uri($"{_organisationUrl}/{orgId}"));
        }

        [Given(@"an Organisation with name (.*) does not exist")]
        public async Task GivenAnOrganisationWithNameOrganisationDoesNotExist(string organisationName)
        {
            var organisations = await OrganisationEntity.FetchAllOrganisationsAsync(_config.GetConnectionString(_configAccessUser));
            organisations.Select(x => x.Name).Should().NotContain(organisationName);
        }

        private void CheckAccessToken()
        {
            if (!_context.ContainsKey(_accessToken))
            {
                _context[_accessToken] = string.Empty;
            }
        }

        private class OrganisationTable
        {
            public string Name { get; set; }

            public string OdsCode { get; set; }
        }
    }
}
