using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        private readonly ScenarioContext context;
        private readonly Config config;

        public UserSteps(ScenarioContext context, Config config)
        {
            this.context = context;
            this.config = config;
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
                await userEntity.InsertAsync(config.ConnectionString);
            }
        }

        private Guid GetOrganisationIdFromName(string organisationName)
        {
            var allOrganisations = context.Get<IDictionary<string, Guid>>(ScenarioContextKeys.OrganisationMapDictionary);
            return allOrganisations.TryGetValue(organisationName, out Guid organisationId) ? organisationId : Guid.Empty;
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
    }
}
