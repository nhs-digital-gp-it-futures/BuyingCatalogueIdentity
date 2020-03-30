using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using IdentityModel.Client;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
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
        private readonly string _organisationUrl;

        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Settings _settings;

        public UserSteps(ScenarioContext context, Response response, Settings settings)
        {
            _context = context;
            _response = response;
            _settings = settings;
            _organisationUrl = settings.OrganisationApiBaseUrl + "/api/v1/Organisations";
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
                        PasswordHash = GenerateHash(user.Password),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Disabled = user.Disabled,
                        PhoneNumber = user.PhoneNumber,
                        Id = user.Id,
                        OrganisationId = organisationId,
                        OrganisationFunction = user.OrganisationFunction,
                        SecurityStamp = "TestUser"
                    };
                
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

            using var client = new HttpClient();
            client.SetBearerToken(_context.Get(ScenarioContextKeys.AccessToken, ""));
            _response.Result = await client.GetAsync(new Uri($"{_organisationUrl}/{organisationId}/users"));
        }

        [When(@"a POST request is made to create a user for organisation (.*)")]
        public async Task WhenAUserIsCreated(string organisationName, Table table)
        {
            var data = table.CreateInstance<CreateUserPostPayload>();
            var organisationId = GetOrganisationIdFromName(organisationName);

            using var client = new HttpClient();
            client.SetBearerToken(_context.Get(ScenarioContextKeys.AccessToken, ""));

            var payload = JsonConvert.SerializeObject(data);
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            _response.Result = await client.PostAsync(new Uri($"{_organisationUrl}/{organisationId}/users"), content);
        }

        private static string GenerateHash(string password)
        {
            const int identityVersion = 1; // 1 = Identity V3
            const int iterationCount = 10000;
            const int passwordHashLength = 32;
            const KeyDerivationPrf hashAlgorithm = KeyDerivationPrf.HMACSHA256;
            const int saltLength = 16;

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[saltLength];
            rng.GetBytes(salt);

            var pbkdf2Hash = KeyDerivation.Pbkdf2(
                password,
                salt,
                hashAlgorithm,
                iterationCount,
                passwordHashLength);

            var identityVersionData = new byte[] { identityVersion };
            var prfData = BitConverter.GetBytes((uint)hashAlgorithm).Reverse().ToArray();
            var iterationCountData = BitConverter.GetBytes((uint)iterationCount).Reverse().ToArray();
            var saltSizeData = BitConverter.GetBytes((uint)saltLength).Reverse().ToArray();

            var hashElements = new[]
            {
                identityVersionData,
                prfData,
                iterationCountData,
                saltSizeData,
                salt,
                pbkdf2Hash
            };

            var identityV3Hash = new List<byte>();
            foreach (var data in hashElements)
            {
                identityV3Hash.AddRange(data);
            }

            identityV3Hash.Count.Should().Be(61);
            return Convert.ToBase64String(identityV3Hash.ToArray());
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
        }

        private async Task<IEnumerable<ExpectedUserTable>> CreateUserTableFromResponse()
        {
            return (await _response.ReadBody()).SelectToken("users").Select(x => new ExpectedUserTable
            {
                UserId = x.SelectToken("userId").ToString(),
                FirstName = x.SelectToken("firstName").ToString(),
                LastName = x.SelectToken("lastName").ToString(),
                EmailAddress = x.SelectToken("emailAddress").ToString(),
                PhoneNumber = x.SelectToken("phoneNumber").ToString(),
                IsDisabled = x.SelectToken("isDisabled").ToString()
            });
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
        }
    }
}
