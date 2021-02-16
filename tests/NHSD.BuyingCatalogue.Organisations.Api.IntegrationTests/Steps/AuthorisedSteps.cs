using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AuthorisedSteps
    {
        private readonly ScenarioContext context;

        public AuthorisedSteps(IConfiguration configuration, ScenarioContext context)
        {
            Configuration = configuration;
            this.context = context;
        }

        private IConfiguration Configuration { get; }

        [Given(@"a user is logged in")]
        public async Task GivenAnUserIsLoggedInWithUsernameAndPassword(Table table)
        {
            var discoveryAddress = Configuration.GetValue<string>("DiscoveryAddress");

            using var client = new HttpClient();

            using var discoveryDocumentRequest = new DiscoveryDocumentRequest
            {
                Policy = new DiscoveryPolicy { RequireHttps = false },
                Address = discoveryAddress,
            };

            var discoveryDocument = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest);

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            var user = table.CreateSet<UserTable>().First();

            using var passwordTokenRequest = new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PasswordClient",
                ClientSecret = "PasswordSecret",
                UserName = user.Username,
                Password = user.Password,
                Scope = user.Scope,
            };

            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(passwordTokenRequest);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            context[ScenarioContextKeys.AccessToken] = tokenResponse.AccessToken;
        }

        [Given(@"the claims contains the following information")]
        public void GivenTheClaimsContainsOrganisation(Table table)
        {
            var expectedClaims = table.Rows.Select(r => (ClaimType: r["ClaimType"], ClaimValue: r["ClaimValue"]));
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(context.Get(ScenarioContextKeys.AccessToken, string.Empty));
            var claims = token.Claims.Select(c => (ClaimType: c.Type, ClaimValue: c.Value));
            claims.Should().Contain(expectedClaims);
        }

        [Then(@"the access token should be empty")]
        public void ThenTheAccessTokenShouldBeEmpty()
        {
            context.Get(ScenarioContextKeys.AccessToken, string.Empty).Should().BeEmpty();
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class UserTable
        {
            public string Username { get; init; }

            public string Password { get; init; }

            public string Scope { get; init; }
        }
    }
}
