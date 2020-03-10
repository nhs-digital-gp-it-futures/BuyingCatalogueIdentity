using System;
using System.Collections.Generic;
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
            var expectedPrimaryOrganisationId = Guid.NewGuid();
            const string expectedOrganisationFunction = "Buyer";

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = expectedUserId,
                    UserName = expectedUserName,
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
                .WithRequestedClaimTypes(
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.PreferredUserName,
                    ApplicationClaimTypes.PrimaryOrganisationId,
                    ApplicationClaimTypes.OrganisationFunction)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var expected = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>(JwtClaimTypes.Subject, expectedUserId),
                new KeyValuePair<string,string>(JwtClaimTypes.PreferredUserName, expectedUserName),
                new KeyValuePair<string,string>(ApplicationClaimTypes.PrimaryOrganisationId, expectedPrimaryOrganisationId.ToString()),
                new KeyValuePair<string,string>(ApplicationClaimTypes.OrganisationFunction, expectedOrganisationFunction)
            };

            var actual = profileDataRequestContext.IssuedClaims.Select(item => new KeyValuePair<string, string>(item.Type, item.Value));
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
