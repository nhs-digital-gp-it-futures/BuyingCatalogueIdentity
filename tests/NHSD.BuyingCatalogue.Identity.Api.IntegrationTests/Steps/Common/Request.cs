using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common
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
        {
            _response.Result = await url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(_context.Get(ScenarioContextKeys.AccessToken, string.Empty))
                .AllowAnyHttpStatus()
                .GetAsync();
        }

        public async Task PostJsonAsync(string url, object payload, params object[] pathSegments)
        {
            _response.Result = await url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(_context.Get(ScenarioContextKeys.AccessToken, string.Empty))
                .AllowAnyHttpStatus()
                .PostJsonAsync(payload);
        }

        public async Task PutJsonAsync(string url, object payload, params object[] pathSegments)
        {
            _response.Result = await url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(_context.Get(ScenarioContextKeys.AccessToken, string.Empty))
                .AllowAnyHttpStatus()
                .PutJsonAsync(payload);
        }
    }
}
