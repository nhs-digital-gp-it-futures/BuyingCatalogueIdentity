using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
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

        private const string AccessTokenKey = "AccessToken";
        private const string OrganisationMapDictionary = "OrganisationMapDictionary";

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
                organisationDictionary.Add(organisation.Name, organisation.Id);
            }

            _context[OrganisationMapDictionary] = organisationDictionary;
        }

        [When(@"a GET request is made for the Organisations section")]
        public async Task WhenAGETRequestIsMadeForTheOrganisationsSection()
        {
            string bearerToken = _context.Get(AccessTokenKey, "");

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
                Name = t.SelectToken("name").ToString(),
                OdsCode = t.SelectToken("odsCode").ToString(),
                PrimaryRoleId = t.SelectToken("primaryRoleId").ToString(),
                CatalogueAgreementSigned = t.SelectToken("catalogueAgreementSigned").ToObject<bool>(),
                Line1 = t.SelectToken("address.line1").ToString(),
                Line2 = t.SelectToken("address.line2").ToString(),
                Line3 = t.SelectToken("address.line3").ToString(),
                Line4 = t.SelectToken("address.line4").ToString(),
                Town = t.SelectToken("address.town").ToString(),
                County = t.SelectToken("address.county").ToString(),
                Postcode = t.SelectToken("address.postcode").ToString(),
                Country = t.SelectToken("address.country").ToString()
            });

            organisations.Should().BeEquivalentTo(expectedOrganisations, options => options.WithStrictOrdering());
        }

        [Then(@"the Organisation is returned with the following values")]
        public async Task ThenTheOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisation = table.CreateSet<OrganisationTable>().FirstOrDefault();

            var org = await _response.ReadBody();
            var organisation = new
            {
                Name = org.SelectToken("name").ToString(),
                OdsCode = org.SelectToken("odsCode").ToString(),
                PrimaryRoleId = org.SelectToken("primaryRoleId").ToString(),
                CatalogueAgreementSigned = org.SelectToken("catalogueAgreementSigned").ToObject<bool>(),
                Line1 = org.SelectToken("address.line1").ToString(),
                Line2 = org.SelectToken("address.line2").ToString(),
                Line3 = org.SelectToken("address.line3").ToString(),
                Line4 = org.SelectToken("address.line4").ToString(),
                Town = org.SelectToken("address.town").ToString(),
                County = org.SelectToken("address.county").ToString(),
                Postcode = org.SelectToken("address.postcode").ToString(),
                Country = org.SelectToken("address.country").ToString()
            };

            organisation.Should().BeEquivalentTo(expectedOrganisation);
        }

        [When(@"a GET request is made for an organisation with name (.*)")]
        public async Task WhenAGETRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(OrganisationMapDictionary);

            var organisationId = Guid.Empty.ToString();
            if (allOrganisations.ContainsKey(organisationName))
            {
                organisationId = allOrganisations?[organisationName].ToString();
            }

            using var client = new HttpClient();
            client.SetBearerToken(_context.Get(AccessTokenKey, ""));
            _response.Result = await client.GetAsync(new Uri($"{_organisationUrl}/{organisationId}"));
        }

        [Given(@"an Organisation with name (.*) does not exist")]
        public async Task GivenAnOrganisationWithNameOrganisationDoesNotExist(string organisationName)
        {
            var organisations = await OrganisationEntity.GetByNameAsync(_settings.ConnectionString, organisationName);
            organisations.Should().BeNull();
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
