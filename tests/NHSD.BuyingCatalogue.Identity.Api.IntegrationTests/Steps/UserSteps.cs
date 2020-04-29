using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UserSteps
    {
        private readonly string _identityOrganisationsUrl;
        private readonly string _identityUsersUrl;

        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;

        public UserSteps(ScenarioContext context, Response response, Request request, Settings settings)
        {
            _context = context;
            _response = response;
            _request = request;
            _settings = settings;
            _identityOrganisationsUrl = settings.IdentityApiBaseUrl + "/api/v1/Organisations";
            _identityUsersUrl = settings.IdentityApiBaseUrl + "/api/v1/users";
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
                await userEntity.InsertAsync(_settings.ConnectionString);
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
        public async Task WhenAGETRequestIsMadeForOrganisationUsersWithName(string organisationName)
        {
            var organisationId = GetOrganisationIdFromName(organisationName);
            await _request.GetAsync(_identityOrganisationsUrl, organisationId, "users");
        }

        [When(@"a POST request is made to create a user for organisation (.*)")]
        public async Task WhenAUserIsCreated(string organisationName, Table table)
        {
            var data = table.CreateInstance<CreateUserPostPayload>();
            var organisationId = GetOrganisationIdFromName(organisationName);

            await _request.PostJsonAsync(_identityOrganisationsUrl, data, organisationId, "users");

            _context[ScenarioContextKeys.EmailSent] = true;
        }

        [When(@"a GET request is made for a user with id (.*)")]
        public async Task WhenAGETRequestIsMadeForAUserWithId(string userId)
        {
            await _request.GetAsync(_identityUsersUrl, userId);
        }

        [Then(@"a user is returned with the following values")]
        public async Task ThenAUserIsReturnedWithTheFollowingValues(Table table)
        {
            var expected = table.CreateInstance<ExpectedGetUserTable>();

            var organisationId = GetOrganisationIdFromName(expected.OrganisationName);

            expected.PrimaryOrganisationId = organisationId;

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new ExpectedGetUserTable
            {
                Name = response.SelectToken("name").ToString(),
                PhoneNumber = response.SelectToken("phoneNumber").ToString(),
                EmailAddress = response.SelectToken("emailAddress").ToString(),
                Disabled = response.SelectToken("disabled").ToObject<bool>(),
                PrimaryOrganisationId = response.SelectToken("primaryOrganisationId").ToObject<Guid>()
            };

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(user => user.OrganisationName));
        }

        [When(@"a POST request is made to (disable|enable) user with id (.*)")]
        public async Task WhenAPOSTRequestIsMadeToChangeTheUsersState(string request, string userId)
        {
            await _request.PostJsonAsync(_identityUsersUrl, null, userId, request);
        }

        [Then(@"the database has user with id (.*)")]
        public async Task ThenTheDatabaseIsUpdatedWithTheUsersNewValues(string userId, Table table)
        {
            var expected = table.CreateInstance<ExpectedGetUserTable>();

            var userEntity = new UserEntity
            {
                Id = userId
            };

            var userInDb = await userEntity.GetAsync(_settings.ConnectionString);

            userInDb.Should().BeEquivalentTo(expected, options => options.Excluding(user => user.PrimaryOrganisationId));
        }

        [Then(@"the response returns a user id")]
        public async Task ThenTheResponseContainsAUserId()
        {
            var response = await _response.ReadBodyAsJsonAsync();

            var actual = response.SelectToken("userId").Value<string>();

            actual.Should().NotBeNull();
        }

        [Then(@"the response contains a valid location header for user with email (.*)")]
        public async Task ThenTheResponseContainsValidLocationHeaderForUser(string email)
        {
            var persistedUserId = await GetUserIdByEmail(email);
            var expected = new Uri($"{_identityUsersUrl}/{persistedUserId}");
            var actual = _response.Result.Headers.Location;
            actual.Should().BeEquivalentTo(expected);
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
        }

        private async Task<IEnumerable<ExpectedUserTable>> CreateUserTableFromResponse()
        {
            return (await _response.ReadBodyAsJsonAsync()).SelectToken("users").Select(x => new ExpectedUserTable
            {
                UserId = x.SelectToken("userId").ToString(),
                FirstName = x.SelectToken("firstName").ToString(),
                LastName = x.SelectToken("lastName").ToString(),
                EmailAddress = x.SelectToken("emailAddress").ToString(),
                PhoneNumber = x.SelectToken("phoneNumber").ToString(),
                IsDisabled = x.SelectToken("isDisabled").ToString()
            });
        }

        private async Task<string> GetUserIdByEmail(string email)
        {
            var userEntity = new UserEntity { Email = email };
            return await userEntity.GetIdByEmail(_settings.ConnectionString);
        }

        private sealed class CreateUserPostPayload
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string PhoneNumber { get; set; }

            public string EmailAddress { get; set; }
        }

        private sealed class ExpectedUserTable
        {
            public string UserId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string EmailAddress { get; set; }

            public string PhoneNumber { get; set; }

            public string IsDisabled { get; set; }
        }

        private sealed class NewUserTable
        {
            public string Password { get; set; } = "Pass123$";

            public string FirstName { get; set; } = "Test";

            public string LastName { get; set; } = "User";

            public string Email { get; set; }

            public string PhoneNumber { get; set; } = "01234567890";

            public bool Disabled { get; set; } = false;

            public string Id { get; set; }

            public string OrganisationName { get; set; }

            public string OrganisationFunction { get; set; } = "Authority";

            public bool CatalogueAgreementSigned { get; set; } = true;
        }

        private sealed class ExpectedGetUserTable
        {
            public string Name { get; set; }

            public string PhoneNumber { get; set; }

            public string EmailAddress { get; set; }

            public bool Disabled { get; set; }

            public string OrganisationName { get; set; }

            public Guid PrimaryOrganisationId { get; set; }
        }
    }
}
