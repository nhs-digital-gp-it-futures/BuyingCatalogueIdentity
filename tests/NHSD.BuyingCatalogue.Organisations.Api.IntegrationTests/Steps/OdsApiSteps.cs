using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using RestEase;
using TechTalk.SpecFlow;
using WireMock.Admin.Mappings;
using WireMock.Client;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OdsApiSteps
    {
        private const string OrganisationsUrl = "/ORD/2-0-0/organisations";

        private readonly ScenarioContext _context;
        private readonly Settings _settings;

        public OdsApiSteps(ScenarioContext context, Settings settings)
        {
            _context = context;
            _settings = settings;
        }

        [Given(@"Ods API is down")]
        public async Task CallToTheOdsApiWillFail()
        {
            await ClearMappings();

            await AddMapping(CreateMappingModel($"{OrganisationsUrl}/*", 500));
        }

        internal async Task SetUpGETChildrenEndpoint(string odsCode, string responseBody)
        {
            await AddMapping(CreateMappingModel(OrganisationsUrl, 200, responseBody));
        }

        internal async Task SetUpGETEndpoint(string odsCode, string responseBody)
        {
            await AddMapping(CreateMappingModel($"{OrganisationsUrl}/{odsCode}", 200, responseBody));
        }

        internal async Task ClearMappings()
        {
            if (_context.Get(ScenarioContextKeys.MappingAdded, false))
            {
                var api = RestClient.For<IWireMockAdminApi>(new Uri(_settings.OdsApiWireMockBaseUrl));
                await api.DeleteMappingsAsync();
                _context[ScenarioContextKeys.MappingAdded] = false;
            }
        }

        private static MappingModel CreateMappingModel(string path, int responseStatusCode, string responseBody = null)
        {
            return new MappingModel
            {
                Response = new ResponseModel { StatusCode = responseStatusCode, Body = responseBody },
                Request = new RequestModel
                {
                    Path = new PathModel
                    {
                        Matchers = new[]
                            {
                                new MatcherModel
                                {
                                    Name = "WildcardMatcher",
                                    Pattern = path,
                                    IgnoreCase = true
                                }
                            }
                    },
                    Methods = new[] { "GET" },
                }
            };
        }

        private async Task AddMapping(MappingModel model)
        {
            var api = RestClient.For<IWireMockAdminApi>(new Uri(_settings.OdsApiWireMockBaseUrl));
            var result = await api.PostMappingAsync(model);
            result.Status.Should().Be("Mapping added");
            _context[ScenarioContextKeys.MappingAdded] = true;
        }
    }
}
