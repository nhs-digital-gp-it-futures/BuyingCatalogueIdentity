using System;
using System.Threading.Tasks;
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

        public async Task GetAsync(Uri url, params object[] pathSegments)
            => _response.Result = (await CreateCommonRequest(url, pathSegments).GetAsync()).ResponseMessage;

        public async Task PostJsonAsync(Uri url, object payload, params object[] pathSegments)
            => _response.Result = (await CreateCommonRequest(url, pathSegments).PostJsonAsync(payload)).ResponseMessage;

        public async Task PutJsonAsync(Uri url, object payload, params object[] pathSegments)
            => _response.Result = (await CreateCommonRequest(url, pathSegments).PutJsonAsync(payload)).ResponseMessage;

        private IFlurlRequest CreateCommonRequest(Uri url, params object[] pathSegments)
        {
            return url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(_context.Get(ScenarioContextKeys.AccessToken, string.Empty))
                .AllowAnyHttpStatus();
        }
    }
}
