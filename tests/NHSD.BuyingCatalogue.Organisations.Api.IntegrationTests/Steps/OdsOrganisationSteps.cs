using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OdsOrganisationSteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly OdsApiSteps _api;

        private readonly string _organisationUrl;

        public OdsOrganisationSteps(Response response, Request request, Settings settings, OdsApiSteps api)
        {
            _response = response;
            _request = request;
            _api = api;
            _organisationUrl = settings.OrganisationsApiBaseUrl + "/api/v1/ods";
        }

        [Given(@"Ods Organisations exist")]
        public async Task GivenOrganisationsExist(Table table)
        {
            var odsOrganisations = table.CreateSet<OdsApiResponseTable>();

            foreach (OdsApiResponseTable org in odsOrganisations)
            {
                var odsApiOrganisation = TransformIntoOdsApiFormat(org);
                var odsApiOrganisationAsJson = JsonConvert.SerializeObject(odsApiOrganisation);
                await _api.SetUpGETEndpoint(org.OdsCode, odsApiOrganisationAsJson);
            }
        }

        [Given(@"Ods Child Organisations exist for organisation (.*)")]
        public async Task GivenChildOrganisationsExist(string odsCode, Table table)
        {
            var odsOrganisations = table.CreateSet<OdsApiResponseTable>().Select(TransformIntoOdsApiChildFormat);
            var odsResponse = new { Organisations = odsOrganisations };
            var odsApiOrganisationAsJson = JsonConvert.SerializeObject(odsResponse);
            await _api.SetUpGETChildrenEndpoint(odsCode, odsApiOrganisationAsJson);
        }

        [When(@"a GET request is made for an Ods organisation with code (.*)")]
        public async Task WhenAGETRequestIsMadeForAnOdsOrganisationWithOdsCode(string odsCode)
        {
            await _request.GetAsync(_organisationUrl, odsCode);
        }

        [Then(@"the Ods Organisation is returned with the following values")]
        public async Task ThenTheOrganisationIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisation = table.CreateSet<OrganisationTable>().FirstOrDefault();

            JToken responseBody = await _response.ReadBodyAsJsonAsync();

            var actualOrganisation = CreateOrganisationFromJson(responseBody);

            actualOrganisation.Should().BeEquivalentTo(expectedOrganisation);
        }

        private static object TransformIntoOdsApiChildFormat(OdsApiResponseTable data)
        {
            return new
            {
                data.Name,
                data.PrimaryRoleId,
                OrgId = data.OdsCode
            };
        }

        private static object TransformIntoOdsApiFormat(OdsApiResponseTable data)
        {
            return new
            {
                Organisation = new
                {
                    data.Name,
                    data.Status,
                    Geoloc = new
                    {
                        Location = new
                        {
                            data.AddrLn1,
                            data.AddrLn2,
                            data.AddrLn3,
                            data.AddrLn4,
                            data.Town,
                            data.County,
                            data.PostCode,
                            data.Country
                        }
                    },
                    Roles = new
                    {
                        Role = new object[]
                        {
                            new
                            {
                                id = data.PrimaryRoleId,
                                primaryRole = true
                            }
                        }
                    }
                }
            };
        }

        private static OrganisationTable CreateOrganisationFromJson(JToken json)
        {
            var getAddressField = new Func<string, string>(s => json.SelectToken("address").Value<string>(s));
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
                Country = getAddressField("country")
            };
        }

        private class OdsApiChildResponseTable
        {
            public string Name { get; set; }
            public string OdsCode { get; set; }
            public string PrimaryRoleId { get; set; }
        }

        private class OdsApiResponseTable
        {
            public string Name { get; set; }
            public string OdsCode { get; set; }
            public string PrimaryRoleId { get; set; }
            public string Status { get; set; }
            public string AddrLn1 { get; set; }
            public string AddrLn2 { get; set; }
            public string AddrLn3 { get; set; }
            public string AddrLn4 { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public string PostCode { get; set; }
            public string Country { get; set; }
        }

        private class OrganisationTable
        {
            public string OdsCode { get; set; }
            public string OrganisationName { get; set; }
            public string PrimaryRoleId { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
            public string Line4 { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public string PostCode { get; set; }
            public string Country { get; set; }
        }
    }
}
