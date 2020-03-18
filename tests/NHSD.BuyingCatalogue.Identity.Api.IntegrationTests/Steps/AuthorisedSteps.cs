using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class AuthorisedSteps
    {
        private readonly ScenarioContext _context;

        public AuthorisedSteps(IConfigurationRoot configuration, ScenarioContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private IConfigurationRoot _configuration { get; }

        [Given(@"an authority user is logged in")]
        public async Task GivenAnAuthorityUserIsLoggedIn()
        {
            var discoveryAddress = _configuration.GetValue<string>("DiscoveryAddress");

            var userName = _configuration.GetValue<string>("UserEmail");
            var userPassword = _configuration.GetValue<string>("UserPassword");
            
            var client = new HttpClient();

            var discoveryDocument =
                await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Policy = new DiscoveryPolicy {RequireHttps = false},
                    Address = discoveryAddress,
                });

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            // request token
            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PasswordClient",
                ClientSecret = "PasswordSecret",
                UserName = userName,
                Password = userPassword,
                Scope = "Organisation"
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
