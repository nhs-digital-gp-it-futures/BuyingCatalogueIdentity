using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrganisationsSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Request _request;
        private readonly Config _config;

        private readonly Uri _organisationUrl;

        public OrganisationsSteps(ScenarioContext context, Response response, Request request, Config config)
        {
            _context = context;
            _response = response;
            _request = request;
            _config = config;

            _organisationUrl = new Uri(_config.OrganisationsApiBaseUrl, "/api/v1/Organisations/");
        }

        [Given(@"Organisations exist")]
        public async Task GivenOrganisationsExist(Table table)
        {
            IDictionary<string, Guid> organisationDictionary = new Dictionary<string, Guid>();

            foreach (var organisationTableItem in table.CreateSet<OrganisationTable>())
            {
                var organisation = OrganisationEntityBuilder
                    .Create()
                    .WithName(organisationTableItem.Name)
                    .WithOdsCode(organisationTableItem.OdsCode)
                    .WithPrimaryRoleId(organisationTableItem.PrimaryRoleId)
                    .WithCatalogueAgreementSigned(organisationTableItem.CatalogueAgreementSigned)

                    .WithAddressLine1(organisationTableItem.Line1)
                    .WithAddressLine2(organisationTableItem.Line2)
                    .WithAddressLine3(organisationTableItem.Line3)
                    .WithAddressLine4(organisationTableItem.Line4)
                    .WithAddressTown(organisationTableItem.Town)
                    .WithAddressCounty(organisationTableItem.County)
                    .WithAddressPostcode(organisationTableItem.Postcode)
                    .WithAddressCountry(organisationTableItem.Country)

                    .Build();

                await organisation.InsertAsync(_config.ConnectionString);
                organisationDictionary.Add(organisation.Name, organisation.OrganisationId);
            }

            _context[ScenarioContextKeys.OrganisationMapDictionary] = organisationDictionary;
        }

        [When(@"a request is made to get a list of organisations")]
        public async Task WhenARequestIsMadeToGetAListOfOrganisations()
        {
            await _request.GetAsync(_organisationUrl);
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisations = table.CreateSet<OrganisationTable>().ToList();

            var organisations = (await _response.ReadBodyAsJsonAsync()).SelectToken("organisations").Select(CreateOrganisation);

            organisations.Should().BeEquivalentTo(expectedOrganisations, options => options.WithStrictOrdering());
        }

        [Then(@"the Organisation is returned with the following values")]
        public async Task ThenTheOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisation = table.CreateSet<OrganisationTable>().FirstOrDefault();

            JToken responseBody = await _response.ReadBodyAsJsonAsync();

            var organisation = CreateOrganisation(responseBody);

            organisation.Should().BeEquivalentTo(expectedOrganisation);
        }

        [When(@"a GET request is made for an organisation with name (.*)")]
        public async Task WhenAGETRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);

            await _request.GetAsync(_organisationUrl, organisationId);
        }

        [When(@"a PUT request is made to update an organisation with name (.*)")]
        public async Task WhenAPUTRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName, Table table)
        {
            var data = table.CreateInstance<UpdateOrganisationPayload>();
            var organisationId = GetOrganisationIdFromName(organisationName);

            await _request.PutJsonAsync(_organisationUrl, data, organisationId);
        }

        [When(@"a POST request is made to create an organisation with values")]
        public async Task WhenAPOSTRequestIsMadeForAnOrganisationWithValues(Table table)
        {
            var data = table.CreateInstance<OrganisationTable>();

            await _request.PostJsonAsync(_organisationUrl, TransformOrganisationIntoPayload(data));

            if (_response.Result.StatusCode == HttpStatusCode.Created)
                await UpdateOrganisationMappingFromResponseBody(data.Name);
        }

        [Given(@"an Organisation with name (.*) does not exist")]
        public async Task GivenAnOrganisationWithNameOrganisationDoesNotExist(string organisationName)
        {
            var organisation = await GetOrganisationEntityByName(organisationName);
            organisation.Should().BeNull();
        }

        [Then(@"the response contains a valid location header for organisation with name (.*)")]
        public async Task ThenTheResponseContainsValidLocationHeaderForUser(string organisationName)
        {
            var persistedOrganisation = await GetOrganisationEntityByName(organisationName);
            var expected = new Uri(_organisationUrl, persistedOrganisation.OrganisationId.ToString());
            var actual = _response.Result.Headers.Location;
            actual.Should().BeEquivalentTo(expected);
        }

        private static object CreateOrganisation(JToken token)
        {
            return new
            {
                Name = token.SelectToken("name").ToString(),
                OdsCode = token.SelectToken("odsCode").ToString(),
                PrimaryRoleId = token.SelectToken("primaryRoleId").ToString(),
                CatalogueAgreementSigned = token.SelectToken("catalogueAgreementSigned").ToObject<bool>(),
                Line1 = token.SelectToken("address.line1").ToString(),
                Line2 = token.SelectToken("address.line2").ToString(),
                Line3 = token.SelectToken("address.line3").ToString(),
                Line4 = token.SelectToken("address.line4").ToString(),
                Town = token.SelectToken("address.town").ToString(),
                County = token.SelectToken("address.county").ToString(),
                Postcode = token.SelectToken("address.postcode").ToString(),
                Country = token.SelectToken("address.country").ToString()
            };
        }

        private static object TransformOrganisationIntoPayload(OrganisationTable data)
        {
            return new
            {
                OrganisationName = data.Name,
                data.OdsCode,
                data.PrimaryRoleId,
                data.CatalogueAgreementSigned,
                Address = new
                {
                    data.Line1,
                    data.Line2,
                    data.Line3,
                    data.Line4,
                    data.Town,
                    data.County,
                    data.Postcode,
                    data.Country
                }
            };
        }

        private async Task<OrganisationEntity> GetOrganisationEntityByName(string name)
        {
            return await OrganisationEntity.GetByNameAsync(_config.ConnectionString, name);
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
        }

        private async Task UpdateOrganisationMappingFromResponseBody(string organisationName)
        {
            var guidAsString = await GetValueFromResponseBody<string>("organisationId");
            var organisationId = Guid.Parse(guidAsString);
            UpdateOrganisationMapping(organisationName, organisationId);
        }

        private async Task<T> GetValueFromResponseBody<T>(string fieldName)
        {
            var response = await _response.ReadBodyAsJsonAsync();
            return response.Value<T>(fieldName);
        }

        private void UpdateOrganisationMapping(string organisationName, Guid organisationId)
        {
            _context.Get(ScenarioContextKeys.OrganisationMapDictionary, new Dictionary<string, Guid>())
                .Add(organisationName, organisationId);
        }

        private class UpdateOrganisationPayload
        {
            public bool CatalogueAgreementSigned { get; set; }
        }

        private class OrganisationTable
        {
            public string Name { get; set; }

            public string OdsCode { get; set; }

            public string PrimaryRoleId { get; set; }

            public bool CatalogueAgreementSigned { get; set; }

            public string Line1 { get; set; }

            public string Line2 { get; set; }

            public string Line3 { get; set; }

            public string Line4 { get; set; }

            public string Town { get; set; }

            public string County { get; set; }

            public string Postcode { get; set; }

            public string Country { get; set; }
        }
    }
}
