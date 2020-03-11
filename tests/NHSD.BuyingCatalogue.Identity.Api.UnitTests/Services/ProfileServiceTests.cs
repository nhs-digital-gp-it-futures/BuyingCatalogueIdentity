using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Constants;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    public sealed class ProfileServiceTests
    {
        [Test]
        public async Task GetProfileDataAsync_GivenAnApplicationUserExists_ReturnsExpectedClaimList()
        {
            const string expectedUserId = "TestUserId";
            const string expectedUserName = "TestUserName";
            const string expectedEmail = "TestUser@Email.com";
            const string expectedFirstName = "Bob";
            const string expectedLastName = "Smith";
            const bool expectedEmailConfirmed = false;
            Guid expectedPrimaryOrganisationId = Guid.NewGuid();
            const string expectedOrganisationFunction = "Authority";
            const string expectedOrganisation = "Manage";

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = expectedUserId,
                    UserName = expectedUserName,
                    FirstName = expectedFirstName,
                    LastName = expectedLastName,
                    Email = expectedEmail,
                    EmailConfirmed = expectedEmailConfirmed,
                    PrimaryOrganisationId = expectedPrimaryOrganisationId,
                    OrganisationFunction = expectedOrganisationFunction
                });

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedUserId)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var expected = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>(JwtClaimTypes.Subject, expectedUserId),
                new KeyValuePair<string,string>(JwtClaimTypes.PreferredUserName, expectedUserName),
                new KeyValuePair<string,string>(JwtRegisteredClaimNames.UniqueName, expectedUserName),
                new KeyValuePair<string,string>(JwtClaimTypes.GivenName, expectedFirstName),
                new KeyValuePair<string,string>(JwtClaimTypes.FamilyName, expectedLastName),
                new KeyValuePair<string,string>(JwtClaimTypes.Name, $"{expectedFirstName} {expectedLastName}"),
                new KeyValuePair<string,string>(JwtClaimTypes.Email, expectedEmail),
                new KeyValuePair<string,string>(JwtClaimTypes.EmailVerified, expectedEmailConfirmed.ToString(CultureInfo.CurrentCulture).ToLowerInvariant()),
                new KeyValuePair<string,string>(ApplicationClaimTypes.PrimaryOrganisationId, expectedPrimaryOrganisationId.ToString()),
                new KeyValuePair<string,string>(ApplicationClaimTypes.OrganisationFunction, expectedOrganisationFunction),
                new KeyValuePair<string,string>(ApplicationClaimTypes.Organisation, expectedOrganisation),
            };

            var actual = profileDataRequestContext.IssuedClaims.Select(item => new KeyValuePair<string, string>(item.Type, item.Value));
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
