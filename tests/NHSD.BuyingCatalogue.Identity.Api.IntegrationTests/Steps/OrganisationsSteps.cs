using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrganisationsSteps
    {
        private readonly ScenarioContext context;
        private readonly Settings settings;

        public OrganisationsSteps(ScenarioContext context, Settings settings)
        {
            this.context = context;
            this.settings = settings;
        }

        public static async Task<OrganisationEntity> GetOrganisationEntityByName(string name, string connectionString)
        {
            return await OrganisationEntity.GetByNameAsync(connectionString, name);
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

                await organisation.InsertAsync(settings.ConnectionString);
                organisationDictionary.Add(organisation.Name, organisation.OrganisationId);
            }

            context[ScenarioContextKeys.OrganisationMapDictionary] = organisationDictionary;
        }

        [Given(@"Organisation (.*) has a Parent Relationship to Organisation (.*)")]
        public async Task GivenOrganisationHasAParentRelationshipToOrganisation(string primaryOrgName, string relatedOrgName)
        {
            var primaryOrganisation = await GetOrganisationEntityByName(primaryOrgName, settings.ConnectionString);
            var relatedOrganisation = await GetOrganisationEntityByName(relatedOrgName, settings.ConnectionString);

            await primaryOrganisation.InsertRelatedOrganisation(settings.ConnectionString, relatedOrganisation.OrganisationId);
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
    }
}
