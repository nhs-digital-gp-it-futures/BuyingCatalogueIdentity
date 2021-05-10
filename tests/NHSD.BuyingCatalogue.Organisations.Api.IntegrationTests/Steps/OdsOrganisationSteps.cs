using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OdsOrganisationSteps
    {
        private readonly Response response;
        private readonly Request request;
        private readonly OdsApiSteps api;

        private readonly Uri organisationUrl;
        private readonly Config config;

        public OdsOrganisationSteps(Response response, Request request, Config config, OdsApiSteps api)
        {
            this.response = response;
            this.request = request;
            this.api = api;
            this.config = config;
            organisationUrl = new Uri(config.OrganisationsApiBaseUrl, "api/v1/ods");
        }

        [Given(@"Ods Organisations exist")]
        public async Task GivenOrganisationsExist(Table table)
        {
            var odsOrganisations = table.CreateSet<OdsApiResponseTable>();

            foreach (OdsApiResponseTable org in odsOrganisations)
            {
                var odsApiOrganisation = TransformIntoOdsApiFormat(org);
                var odsApiOrganisationAsJson = JsonConvert.SerializeObject(odsApiOrganisation);
                await api.SetUpGetEndpoint(org.OdsCode, odsApiOrganisationAsJson);

                var organisation = OrganisationEntityBuilder
                    .Create()
                    .WithId(org.Id)
                    .WithName(org.Name)
                    .WithOdsCode(org.OdsCode)
                    .WithAddressLine1(org.AddressLine1)
                    .WithAddressLine2(org.AddressLine2)
                    .WithAddressLine3(org.AddressLine3)
                    .WithAddressLine4(org.AddressLine4)
                    .WithAddressTown(org.Town)
                    .WithAddressCounty(org.County)
                    .WithAddressPostcode(org.PostCode)
                    .WithAddressCountry(org.Country)
                    .Build();
                await organisation.InsertAsync(config.ConnectionString);
            }
        }

        [Given(@"Ods Child Organisations exist for organisation .*")]
        public async Task GivenChildOrganisationsExist(Table table)
        {
            var odsOrganisations = table.CreateSet<OdsApiResponseTable>().Select(TransformIntoOdsApiChildFormat);
            var odsResponse = new { Organisations = odsOrganisations };
            var odsApiOrganisationAsJson = JsonConvert.SerializeObject(odsResponse);
            await api.SetUpGetChildrenEndpoint(odsApiOrganisationAsJson);
        }

        [When(@"a GET request is made for an Ods organisation with code (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOdsOrganisationWithOdsCode(string odsCode)
        {
            await request.GetAsync(organisationUrl, odsCode);
        }

        [Then(@"the Ods Organisation is returned with the following values")]
        public async Task ThenTheOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisation = table.CreateSet<OrganisationTable>().FirstOrDefault();

            JToken responseBody = await response.ReadBodyAsJsonAsync();

            var actualOrganisation = CreateOrganisationFromJson(responseBody);

            actualOrganisation.Should().BeEquivalentTo(expectedOrganisation);
        }

        private static object TransformIntoOdsApiChildFormat(OdsApiResponseTable data)
        {
            return new
            {
                data.Name,
                data.PrimaryRoleId,
                OrgId = data.OdsCode,
            };
        }

        private static object TransformIntoOdsApiFormat(OdsApiResponseTable data)
        {
            return new
            {
                Organisation = new
                {
                    data.Id,
                    data.Name,
                    data.Status,
                    Geoloc = new
                    {
                        Location = new
                        {
                            AddrLn1 = data.AddressLine1,
                            AddrLn2 = data.AddressLine2,
                            AddrLn3 = data.AddressLine3,
                            AddrLn4 = data.AddressLine4,
                            data.Town,
                            data.County,
                            data.PostCode,
                            data.Country,
                        },
                    },
                    Roles = new
                    {
                        Role = new object[]
                        {
                            new
                            {
                                id = data.PrimaryRoleId,
                                primaryRole = true,
                            },
                        },
                    },
                },
            };
        }

        private static OrganisationTable CreateOrganisationFromJson(JToken json)
        {
            var getAddressField = new Func<string, string>(s => json.SelectToken("address")?.Value<string>(s));
            return new OrganisationTable
            {
                OdsCode = json.Value<string>("odsCode"),
                OrganisationName = json.Value<string>("organisationName"),
                PrimaryRoleId = json.Value<string>("primaryRoleId"),
                Line1 = getAddressField("line1"),
                Line2 = getAddressField("line2"),
                Line3 = getAddressField("line3"),
                Line4 = getAddressField("line4"),
                Town = getAddressField("town"),
                County = getAddressField("county"),
                PostCode = getAddressField("postcode"),
                Country = getAddressField("country"),
            };
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OdsApiResponseTable
        {
            public Guid Id { get; set; }

            public string Name { get; init; }

            public string OdsCode { get; init; }

            public string PrimaryRoleId { get; init; }

            public string Status { get; init; }

            public string AddressLine1 { get; init; }

            public string AddressLine2 { get; init; }

            public string AddressLine3 { get; init; }

            public string AddressLine4 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string PostCode { get; init; }

            public string Country { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class OrganisationTable
        {
            public string OdsCode { get; init; }

            public string OrganisationName { get; init; }

            public string PrimaryRoleId { get; init; }

            public string Line1 { get; init; }

            public string Line2 { get; init; }

            public string Line3 { get; init; }

            public string Line4 { get; init; }

            public string Town { get; init; }

            public string County { get; init; }

            public string PostCode { get; init; }

            public string Country { get; init; }
        }
    }
}
