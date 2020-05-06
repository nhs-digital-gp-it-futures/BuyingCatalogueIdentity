using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AuthorisedSteps
    {
        private readonly ScenarioContext _context;
        private IConfiguration _configuration { get; }

        public AuthorisedSteps(IConfiguration configuration, ScenarioContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [Given(@"a user is logged in")]
        public async Task GivenAnUserIsLoggedInWithUsernameAndPassword(Table table)
        {
            var discoveryAddress = _configuration.GetValue<string>("DiscoveryAddress");

            using var client = new HttpClient();

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

            var user = table.CreateSet<UserTable>().First();

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
            var token = handler.ReadJwtToken(_context.Get(ScenarioContextKeys.AccessToken, string.Empty));
            var claims = token.Claims.Select(x => (ClaimType: x.Type, ClaimValue: x.Value));
            claims.Should().Contain(expectedClaims);
        }

        [Then(@"the access token should be empty")]
        public void ThenTheAccessTokenShouldBeEmpty()
        {
            _context.Get(ScenarioContextKeys.AccessToken, string.Empty).Should().BeEmpty();
        }

        private sealed class UserTable
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Scope { get; set; }
        }
    }
}
