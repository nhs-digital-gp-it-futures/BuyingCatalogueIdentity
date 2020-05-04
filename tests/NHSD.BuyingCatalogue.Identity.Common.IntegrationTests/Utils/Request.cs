﻿using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils
{
    internal sealed class Request
    {
        private readonly Response _response;
        private readonly ScenarioContext _context;

        public Request(Response response, ScenarioContext context)
        {
            _response = response;
            _context = context;
        }

        public async Task GetAsync(string url, params object[] pathSegments)
            => _response.Result = await CreateCommonRequest(url, pathSegments).GetAsync();

        public async Task PostJsonAsync(string url, object payload, params object[] pathSegments)
            => _response.Result = await CreateCommonRequest(url, pathSegments).PostJsonAsync(payload);

        public async Task PutJsonAsync(string url, object payload, params object[] pathSegments)
            => _response.Result = await CreateCommonRequest(url, pathSegments).PutJsonAsync(payload);

        private IFlurlRequest CreateCommonRequest(string url, params object[] pathSegments)
        {
            return url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(_context.Get(ScenarioContextKeys.AccessToken, string.Empty))
                .AllowAnyHttpStatus();
        }
    }
}
