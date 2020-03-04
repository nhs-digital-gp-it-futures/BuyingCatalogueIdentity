using System;
using System.Net.Http;
using System.Reflection.Metadata;
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
                await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest{ Address = "http://localhost:8070", });

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "SampleClient",
                ClientSecret = "SampleClientSecret",
                Scope = "SampleResource"
            });

            var tokenResponsew = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,

                ClientId = "SampleClient",
                ClientSecret = "SampleClientSecret",
                Scope = "ClientCredentials",

                UserName = "alice",
                Password = "Pass123$"
            });

            //var a = await client.RequestTokenAsync(new TokenRequest() {
            //    Address = discoveryDocument.TokenEndpoint,
            //    GrantType = "Code",

            //    ClientId = "SampleClient",
            //    ClientSecret = "SampleClientSecret",
            //});

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
        }

    }
}
