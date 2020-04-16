using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using RestEase;
using TechTalk.SpecFlow;
using WireMock.Admin.Mappings;
using WireMock.Client;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class OdsApiSteps
    {
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
            var model = CreateMappingModel("/ORD/2-0-0/organisations/*", 500);

            await AddMapping(model);
        }

        public async Task SetUpGETEndpoint(string odsCode, string responseBody)
        {
            var model = CreateMappingModel($"/ORD/2-0-0/organisations/{odsCode}", 200, responseBody);

            await AddMapping(model);
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
            model.Priority = 10;
            var api = RestClient.For<IWireMockAdminApi>(new Uri(_settings.OdsApiWireMockBaseUrl));
            var result = await api.PostMappingAsync(model);
            result.Status.Should().Be("Mapping added");
            _context[ScenarioContextKeys.MappingAdded] = true;
        }

        public async Task ClearMappings()
        {
            if (_context.Get(ScenarioContextKeys.MappingAdded, false))
            {
                var api = RestClient.For<IWireMockAdminApi>(new Uri(_settings.OdsApiWireMockBaseUrl));
                await api.DeleteMappingsAsync();
                _context[ScenarioContextKeys.MappingAdded] = false;
            }
        }
    }
}
