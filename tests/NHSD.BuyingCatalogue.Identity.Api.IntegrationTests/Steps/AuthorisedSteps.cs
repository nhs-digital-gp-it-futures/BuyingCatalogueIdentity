using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class AuthorisedSteps
    {
        private readonly ScenarioContext _context;
        private IConfigurationRoot _configuration { get; }

        public AuthorisedSteps(IConfigurationRoot configuration, ScenarioContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        

        [Given(@"an user is logged in")]
        public async Task GivenAnUserIsLoggedInWithUsernameAndPassword(Table table)
        {
            var user = table.CreateSet<UserTable>().First();

            var discoveryAddress = _configuration.GetValue<string>("DiscoveryAddress");

            var client = new HttpClient();

            var discoveryDocument =
                await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Policy = new DiscoveryPolicy { RequireHttps = false },
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
                UserName = user.Username,
                Password = user.Password,
                Scope = user.Scope
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            _context[ScenarioContextKeys.AccessToken] = tokenResponse.AccessToken;
        }

        [Given(@"the claims contains the following information")]
        public void GivenTheClaimsContainsOrganisation(Table table)
        {
            var expectedClaims = table.Rows.Select(x => (ClaimType: x["ClaimType"], ClaimValue: x["ClaimValue"]));
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(_context.Get(ScenarioContextKeys.AccessToken, ""));
            var claims = token.Claims.Select(x => (ClaimType: x.Type, ClaimValue: x.Value));
            claims.Should().Contain(expectedClaims);
        }

        [Then(@"the access token should be empty")]
        public void ThenTheAccessTokenShouldBeEmpty()
        {
            _context.Get(ScenarioContextKeys.AccessToken, "").Should().BeEmpty();
        }

        private sealed class UserTable
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Scope { get; set; }
        }
    }
}
