using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class AuthorisedSteps
    {
        private readonly ScenarioContext _context;

        public AuthorisedSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"an authority user is logged in")]
        public async Task GivenAnAuthorityUserIsLoggedIn()
        {
            var client = new HttpClient();

            var discoveryDocument =
                await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest{ Address = "http://docker.for.win.localhost:8070/", });

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "TokenClient",
                ClientSecret = "TokenSecret",
                Scope = "SampleResource"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            _context["AccessToken"] = tokenResponse.AccessToken;
        }
    }
}
