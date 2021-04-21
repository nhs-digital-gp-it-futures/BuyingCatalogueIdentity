using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
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
        private readonly ScenarioContext context;
        private readonly Response response;
        private readonly Request request;
        private readonly Config config;

        private readonly Uri organisationUrl;
        private readonly string relatedOrganisationsPath;
        private readonly string unrelatedOrganisationsPath;

        public OrganisationsSteps(ScenarioContext context, Response response, Request request, Config config)
        {
            this.context = context;
            this.response = response;
            this.request = request;
            this.config = config;

            organisationUrl = new Uri(config.OrganisationsApiBaseUrl, "/api/v1/Organisations/");
            relatedOrganisationsPath = "related-organisations";
            unrelatedOrganisationsPath = "unrelated-organisations";
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

                await organisation.InsertAsync(config.ConnectionString);
                organisationDictionary.Add(organisation.Name, organisation.OrganisationId);
            }

            context[ScenarioContextKeys.OrganisationMapDictionary] = organisationDictionary;
        }

        [When(@"a request is made to get a list of organisations")]
        public async Task WhenARequestIsMadeToGetAListOfOrganisations()
        {
            await request.GetAsync(organisationUrl);
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisations = table.CreateSet<OrganisationTable>().ToList();

            var organisations = (await response.ReadBodyAsJsonAsync()).SelectToken("organisations")?.Select(CreateOrganisation);

            organisations.Should().BeEquivalentTo(expectedOrganisations, options => options.WithStrictOrdering());
        }

        [Then(@"the Organisation is returned with the following values")]
        public async Task ThenTheOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisation = table.CreateSet<OrganisationTable>().FirstOrDefault();

            JToken responseBody = await response.ReadBodyAsJsonAsync();

            var organisation = CreateOrganisation(responseBody);

            organisation.Should().BeEquivalentTo(expectedOrganisation);
        }

        [When(@"a GET request is made for an organisation with name (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);

            await request.GetAsync(organisationUrl, organisationId);
        }

        [When(@"a PUT request is made to update an organisation with name (.*)")]
        public async Task WhenAPutRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName, Table table)
        {
            var data = table.CreateInstance<UpdateOrganisationPayload>();
            var organisationId = GetOrganisationIdFromName(organisationName);

            await request.PutJsonAsync(organisationUrl, data, organisationId);
        }

        [When(@"a POST request is made to create an organisation with values")]
        public async Task WhenAPostRequestIsMadeForAnOrganisationWithValues(Table table)
        {
            var data = table.CreateInstance<OrganisationTable>();

            await request.PostJsonAsync(organisationUrl, TransformOrganisationIntoPayload(data));

            if (response.Result.StatusCode == HttpStatusCode.Created)
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
            var expected = new Uri(organisationUrl, persistedOrganisation.OrganisationId.ToString());
            var actual = response.Result.Headers.Location;
            actual.Should().BeEquivalentTo(expected);
        }

        [Given(@"Organisation (.*) has a Parent Relationship to Organisation (.*)")]
        public async Task GivenOrganisationHasAParentRelationshipToOrganisation(string primaryOrgName, string relatedOrgName)
        {
            var primaryOrganisation = await GetOrganisationEntityByName(primaryOrgName);
            var relatedOrganisation = await GetOrganisationEntityByName(relatedOrgName);

            await primaryOrganisation.InsertRelatedOrganisation(config.ConnectionString, relatedOrganisation.OrganisationId);
        }

        [When(@"a GET request for RelatedOrganisations is made for an Organisation with name (.*)")]
        public async Task WhenAGetRequestForRelatedOrganisationsIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);

            await request.GetAsync(organisationUrl, organisationId, relatedOrganisationsPath);
        }

        [When(@"a GET request for UnrelatedOrganisations is made for an Organisation with name (.*)")]
        public async Task WhenAGetRequestForUnrelatedOrganisationsIsMadeForAnOrganisationWithNameOrganisations(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);

            await request.GetAsync(organisationUrl, organisationId, unrelatedOrganisationsPath);
        }

        [When(@"a POST request to RelatedOrganisations is made to add an Organisation with name (.*) as a child of an Organisation with name (.*)")]
        public async Task WhenAPostRequestForRelatedOrganisationsIsMadeToAddAnOrganisationWithNameOrganisationsAsAChildOfAnOrganisationWithNameOrganisation(string childOrganisationName, string parentOrganisationName)
        {
            var parentOrganisationId = GetOrganisationIdFromName(parentOrganisationName);

            var childOrganisationId = GetOrganisationIdFromName(childOrganisationName);

            await request.PostJsonAsync(organisationUrl, TransformRelatedOrganisationIdIntoPayload(childOrganisationId), parentOrganisationId, relatedOrganisationsPath);
        }

        [When(@"a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name (.*) and a child Organisation with name (.*)")]
        public async Task WhenADeleteRequestToRelatedOrganisationsIsMadeToDeleteTheRelationshipBetweenAParentOrganisationWithNameOrganisationAndAChildOrganisationWithNameOrganisation(string parentOrganisationName, string childOrganisationName)
        {
            var parentOrganisationId = GetOrganisationIdFromName(parentOrganisationName);

            var childOrganisationId = GetOrganisationIdFromName(childOrganisationName);

            await request.DeleteAsync(organisationUrl, parentOrganisationId, relatedOrganisationsPath, childOrganisationId);
        }

        [Then(@"the RelatedOrganisation is returned with the following values")]
        public async Task ThenTheRelatedOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedRelatedOrganisation = table.CreateSet<RelatedOrganisationTable>().FirstOrDefault();

            JToken responseBody = (await response.ReadBodyAsJsonAsync())?.FirstOrDefault();

            var organisation = CreateRelatedOrganisation(responseBody);

            organisation.Should().BeEquivalentTo(expectedRelatedOrganisation, options => options.WithStrictOrdering());
        }

        [Then(@"a list of Related Organisations is returned with the following values")]
        public async Task ThenAListOfRelatedOrganisationsIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedUnrelatedOrganisations = table.CreateSet<RelatedOrganisationTable>().ToList();

            JToken responseBody = await response.ReadBodyAsJsonAsync();

            var organisations = (await response.ReadBodyAsJsonAsync())?.Select(CreateRelatedOrganisation);

            organisations.Should().BeEquivalentTo(expectedUnrelatedOrganisations, options => options.WithStrictOrdering());
        }

        [Then(@"a list of Related Organisations is returned that does not contain the following values")]
        public async Task ThenAListOfRelatedOrganisationsIsReturnedThatDoesNotContainTheFollowingValues(Table table)
        {
            var unexpectedRelatedOrganisations = table.CreateSet<RelatedOrganisationTable>().ToList();

            JToken responseBody = await response.ReadBodyAsJsonAsync();

            var organisations = (await response.ReadBodyAsJsonAsync())?.Select(CreateRelatedOrganisation);

            organisations.Should().NotContain(unexpectedRelatedOrganisations);
        }

        private static object CreateOrganisation(JToken token)
        {
            return new
            {
                Name = token.SelectToken("name")?.ToString(),
                OdsCode = token.SelectToken("odsCode")?.ToString(),
                PrimaryRoleId = token.SelectToken("primaryRoleId")?.ToString(),
                CatalogueAgreementSigned = token.SelectToken("catalogueAgreementSigned")?.ToObject<bool>(),
                Line1 = token.SelectToken("address.line1")?.ToString(),
                Line2 = token.SelectToken("address.line2")?.ToString(),
                Line3 = token.SelectToken("address.line3")?.ToString(),
                Line4 = token.SelectToken("address.line4")?.ToString(),
                Town = token.SelectToken("address.town")?.ToString(),
                County = token.SelectToken("address.county")?.ToString(),
                Postcode = token.SelectToken("address.postcode")?.ToString(),
                Country = token.SelectToken("address.country")?.ToString(),
            };
        }

        private static object CreateRelatedOrganisation(JToken token)
        {
            return new
            {
                Name = token.SelectToken("name")?.ToString(),
                OdsCode = token.SelectToken("odsCode")?.ToString(),
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
                    data.Country,
                },
            };
        }

        private static object TransformRelatedOrganisationIdIntoPayload(Guid childOrganisationId)
        {
            return new
            {
                relatedOrganisationId = childOrganisationId,
            };
        }

        private async Task<OrganisationEntity> GetOrganisationEntityByName(string name)
        {
            return await OrganisationEntity.GetByNameAsync(config.ConnectionString, name);
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
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
            var jsonBody = await response.ReadBodyAsJsonAsync();
            return jsonBody.Value<T>(fieldName);
        }

        private void UpdateOrganisationMapping(string organisationName, Guid organisationId)
        {
            context.Get(ScenarioContextKeys.OrganisationMapDictionary, new Dictionary<string, Guid>())
                .Add(organisationName, organisationId);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class UpdateOrganisationPayload
        {
            public bool CatalogueAgreementSigned { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrganisationTable
        {
            public string Name { get; init; }

            public string OdsCode { get; init; }

            public string PrimaryRoleId { get; init; }

            public bool CatalogueAgreementSigned { get; init; }

            public string Line1 { get; init; }

            public string Line2 { get; init; }

            public string Line3 { get; init; }

            public string Line4 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string Postcode { get; init; }

            public string Country { get; init; }
        }

        private sealed class RelatedOrganisationTable
        {
            public string Name { get; init; }

            public string OdsCode { get; init; }
        }
    }
}
