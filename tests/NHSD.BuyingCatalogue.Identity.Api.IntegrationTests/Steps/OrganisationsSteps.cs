using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
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
        private readonly Settings _settings;

        private readonly string _organisationUrl;

        public OrganisationsSteps(ScenarioContext context, Response response, Settings settings)
        {
            _context = context;
            _response = response;
            _settings = settings;

            _organisationUrl = _settings.OrganisationApiBaseUrl + "/api/v1/Organisations";
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

                await organisation.InsertAsync(_settings.ConnectionString);
                organisationDictionary.Add(organisation.Name, organisation.OrganisationId);
            }

            _context[ScenarioContextKeys.OrganisationMapDictionary] = organisationDictionary;
        }

        [When(@"a GET request is made for the Organisations section")]
        public async Task WhenAGETRequestIsMadeForTheOrganisationsSection()
        {
            string bearerToken = _context.Get(ScenarioContextKeys.AccessTokenKey, "");

            using var client = new HttpClient();
            client.SetBearerToken(bearerToken);

            _response.Result = await client.GetAsync(new Uri(_organisationUrl));
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisations = table.CreateSet<OrganisationTable>().ToList();

            var organisations = (await _response.ReadBody()).SelectToken("organisations").Select(CreateOrganisation);

            organisations.Should().BeEquivalentTo(expectedOrganisations, options => options.WithStrictOrdering());
        }

        [Then(@"the Organisation is returned with the following values")]
        public async Task ThenTheOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisation = table.CreateSet<OrganisationTable>().FirstOrDefault();

            JToken responseBody = await _response.ReadBody();

            var organisation = CreateOrganisation(responseBody);

            organisation.Should().BeEquivalentTo(expectedOrganisation);
        }

        [When(@"a GET request is made for an organisation with name (.*)")]
        public async Task WhenAGETRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);

            var organisationId = Guid.Empty.ToString();
            if (allOrganisations.ContainsKey(organisationName))
            {
                organisationId = allOrganisations?[organisationName].ToString();
            }

            using var client = new HttpClient();
            client.SetBearerToken(_context.Get(ScenarioContextKeys.AccessTokenKey, ""));
            _response.Result = await client.GetAsync(new Uri($"{_organisationUrl}/{organisationId}"));
        }

        [Given(@"an Organisation with name (.*) does not exist")]
        public async Task GivenAnOrganisationWithNameOrganisationDoesNotExist(string organisationName)
        {
            var organisations = await OrganisationEntity.GetByNameAsync(_settings.ConnectionString, organisationName);
            organisations.Should().BeNull();
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
