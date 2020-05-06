using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Organisations.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class UserSteps
    {
        private readonly ScenarioContext _context;
        private readonly Settings _settings;

        public UserSteps(ScenarioContext context, Settings settings)
        {
            _context = context;
            _settings = settings;
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

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
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
    }
}
