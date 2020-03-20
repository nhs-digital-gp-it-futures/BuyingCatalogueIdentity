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
        private readonly ContextConstants _contextConstants;
        private IConfigurationRoot _configuration { get; }

        public AuthorisedSteps(IConfigurationRoot configuration, ScenarioContext context)
        {
            _configuration = configuration;
            _context = context;
            _contextConstants = new ContextConstants();
        }
        

        [Given(@"an authority user is logged in")]
        public async Task GivenAnAuthorityUserIsLoggedInWithUsernameAndPassword(Table table)
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

            _context[_contextConstants.AccessTokenKey] = tokenResponse.AccessToken;
        }

        [Given(@"the claims contains the following information")]
        public void GivenTheClaimsContainsOrganisation(Table table)
        {
            var expectedClaims = table.Rows.First().Select(x => (x.Key, x.Value));
            
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(_context.Get(_contextConstants.AccessTokenKey, ""));
            var claims = token.Claims.Select(x => (x.Type, x.Value));

            claims.Should().Contain(expectedClaims);
        }

        [Then(@"the access token should be empty")]
        public void ThenTheAccessTokenShouldBeEmpty()
        {
            _context.Get(_contextConstants.AccessTokenKey, "").Should().BeEmpty();
        }

        private class UserTable
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Scope { get; set; }
        }
    }
}
