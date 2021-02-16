using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UserSteps
    {
        private readonly Uri identityOrganisationsUrl;
        private readonly Uri identityUsersUrl;

        private readonly ScenarioContext context;
        private readonly Response response;
        private readonly Request request;
        private readonly Settings settings;

        public UserSteps(ScenarioContext context, Response response, Request request, Settings settings)
        {
            this.context = context;
            this.response = response;
            this.request = request;
            this.settings = settings;
            identityOrganisationsUrl = new Uri(settings.IdentityApiBaseUrl, "api/v1/Organisations");
            identityUsersUrl = new Uri(settings.IdentityApiBaseUrl, "api/v1/users");
        }

        [Given(@"Users exist")]
        public async Task GivenUsersExist(Table table)
        {
            var users = table.CreateSet<NewUserTable>();
            foreach (var user in users)
            {
                var organisationId = GetOrganisationIdFromName(user.OrganisationName);
                var userEntity = new UserEntity
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Disabled = user.Disabled,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id,
                    OrganisationId = organisationId,
                    OrganisationFunction = user.OrganisationFunction,
                    SecurityStamp = "TestUser",
                    CatalogueAgreementSigned = user.CatalogueAgreementSigned,
                };
                userEntity.PasswordHash = new PasswordHasher<UserEntity>().HashPassword(userEntity, user.Password);
                await userEntity.InsertAsync(settings.ConnectionString);
            }
        }

        [Then(@"the Users list is returned with the following values")]
        public async Task ThenTheUsersListIsReturnedWithValues(Table table)
        {
            var expected = table.CreateSet<ExpectedUserTable>();
            var actual = await CreateUserTableFromResponse();

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the Users list is returned with the following values excluding ID")]
        public async Task ThenTheUsersListIsReturnedWithRealValues(Table table)
        {
            var expected = table.CreateSet<ExpectedUserTable>();
            var actual = await CreateUserTableFromResponse();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(user => user.UserId));
        }

        [When(@"a GET request is made for an organisation's users with name (.*)")]
        public async Task WhenAGetRequestIsMadeForOrganisationUsersWithName(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);
            await request.GetAsync(identityOrganisationsUrl, organisationId, "users");
        }

        [When(@"a POST request is made to create a user for organisation (.*)")]
        public async Task WhenAUserIsCreated(string organisationName, Table table)
        {
            var data = table.CreateInstance<CreateUserPostPayload>();
            var organisationId = GetOrganisationIdFromName(organisationName);

            await request.PostJsonAsync(identityOrganisationsUrl, data, organisationId, "users");

            context[ScenarioContextKeys.EmailSent] = true;
        }

        [When(@"a GET request is made for a user with id (.*)")]
        public async Task WhenAGetRequestIsMadeForAUserWithId(string userId)
        {
            await request.GetAsync(identityUsersUrl, userId);
        }

        [Then(@"a user is returned with the following values")]
        public async Task ThenAUserIsReturnedWithTheFollowingValues(Table table)
        {
            var expected = table.CreateInstance<ExpectedGetUserTable>();

            var organisationId = GetOrganisationIdFromName(expected.OrganisationName);

            expected.PrimaryOrganisationId = organisationId;

            var jsonBody = await response.ReadBodyAsJsonAsync();

            var actual = new ExpectedGetUserTable
            {
                Name = jsonBody.SelectToken("name")?.ToString(),
                PhoneNumber = jsonBody.SelectToken("phoneNumber")?.ToString(),
                EmailAddress = jsonBody.SelectToken("emailAddress")?.ToString(),
                Disabled = jsonBody.SelectToken("disabled")?.ToObject<bool>(),
                PrimaryOrganisationId = jsonBody.SelectToken("primaryOrganisationId")?.ToObject<Guid>(),
            };

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(user => user.OrganisationName));
        }

        [When(@"a POST request is made to (disable|enable) user with id (.*)")]
        public async Task WhenAPostRequestIsMadeToChangeTheUsersState(string postRequest, string userId)
        {
            await request.PostJsonAsync(identityUsersUrl, null, userId, postRequest);
        }

        [Then(@"the database has user with id (.*)")]
        public async Task ThenTheDatabaseIsUpdatedWithTheUsersNewValues(string userId, Table table)
        {
            var expected = table.CreateInstance<ExpectedGetUserTable>();

            var userEntity = new UserEntity { Id = userId };

            var userInDb = await userEntity.GetAsync(settings.ConnectionString);

            userInDb.Should().BeEquivalentTo(expected, options => options.Excluding(user => user.PrimaryOrganisationId));
        }

        [Then(@"the response returns a user id")]
        public async Task ThenTheResponseContainsAUserId()
        {
            var jsonBody = await response.ReadBodyAsJsonAsync();

            var actual = jsonBody.SelectToken("userId")?.Value<string>();

            actual.Should().NotBeNull();
        }

        [Then(@"the response contains a valid location header for user with email (.*)")]
        public async Task ThenTheResponseContainsValidLocationHeaderForUser(string email)
        {
            var persistedUserId = await GetUserIdByEmail(email);
            var expected = new Uri($"{identityUsersUrl}/{persistedUserId}");
            var actual = response.Result.Headers.Location;
            actual.Should().BeEquivalentTo(expected);
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
        }

        private async Task<IEnumerable<ExpectedUserTable>> CreateUserTableFromResponse()
        {
            return (await response.ReadBodyAsJsonAsync()).SelectToken("users")?.Select(t => new ExpectedUserTable
            {
                UserId = t.SelectToken("userId")?.ToString(),
                FirstName = t.SelectToken("firstName")?.ToString(),
                LastName = t.SelectToken("lastName")?.ToString(),
                EmailAddress = t.SelectToken("emailAddress")?.ToString(),
                PhoneNumber = t.SelectToken("phoneNumber")?.ToString(),
                IsDisabled = t.SelectToken("isDisabled")?.ToString(),
            });
        }

        private async Task<string> GetUserIdByEmail(string email)
        {
            var userEntity = new UserEntity { Email = email };
            return await userEntity.GetIdByEmail(settings.ConnectionString);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class CreateUserPostPayload
        {
            public string FirstName { get; init; }

            public string LastName { get; init; }

            public string PhoneNumber { get; init; }

            public string EmailAddress { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ExpectedUserTable
        {
            public string UserId { get; init; }

            public string FirstName { get; init; }

            public string LastName { get; init; }

            public string EmailAddress { get; init; }

            public string PhoneNumber { get; init; }

            public string IsDisabled { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class NewUserTable
        {
            public string Password { get; init; } = "Pass123$";

            public string FirstName { get; init; } = "Test";

            public string LastName { get; init; } = "User";

            public string Email { get; init; }

            public string PhoneNumber { get; init; } = "01234567890";

            public bool Disabled { get; init; }

            public string Id { get; init; }

            public string OrganisationName { get; init; }

            public string OrganisationFunction { get; init; } = "Authority";

            public bool CatalogueAgreementSigned { get; init; } = true;
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class ExpectedGetUserTable
        {
            public string Name { get; init; }

            public string PhoneNumber { get; init; }

            public string EmailAddress { get; init; }

            public bool? Disabled { get; init; }

            public string OrganisationName { get; init; }

            public Guid? PrimaryOrganisationId { get; set; }
        }
    }
}
