using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;
using static IdentityModel.JwtClaimTypes;
using static NHSD.BuyingCatalogue.Identity.Common.Constants.ApplicationClaimTypes;
using static NHSD.BuyingCatalogue.Identity.Common.Constants.ApplicationPermissions;
namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class ProfileServiceTests
    {
        [Test]
        public async Task GetProfileDataAsync_GivenAnApplicationUserExists_ReturnsExpectedClaimList()
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithEmail("TestUser@Email.com")
                .WithFirstName("Bob")
                .WithLastName("Smith")
                .WithOrganisationFunction("Authority")
                .Build();

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var expected = new List<(string, string)>
            {
                (Subject, expectedApplicationUser.Id),
                (PreferredUserName, expectedApplicationUser.UserName),
                (JwtRegisteredClaimNames.UniqueName, expectedApplicationUser.UserName),
                (GivenName, expectedApplicationUser.FirstName),
                (FamilyName, expectedApplicationUser.LastName),
                (Name, $"{expectedApplicationUser.FirstName} {expectedApplicationUser.LastName}"),
                (Email, expectedApplicationUser.Email),
                (EmailVerified, expectedApplicationUser.EmailConfirmed.ToString(CultureInfo.CurrentCulture).ToLowerInvariant()),
                (PrimaryOrganisationId, expectedApplicationUser.PrimaryOrganisationId.ToString()),
                (OrganisationFunction, expectedApplicationUser.OrganisationFunction),
                (Organisation, Manage),
                (Account, Manage)
            };

            var actual = profileDataRequestContext.IssuedClaims.Select(item => (item.Type, item.Value));
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("SomeId", "SomeUserName", Subject, PreferredUserName, JwtRegisteredClaimNames.UniqueName, PrimaryOrganisationId)]
        [TestCase("SomeId", null, Subject, PrimaryOrganisationId)]
        public async Task GetProfileDataAsync_GivenApplicationUserWithId_ReturnExpectedClaimList(
            string expectedUserId,
            string expectedUserName,
            params string[] expectedClaimTypes)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithId(expectedUserId)
                .WithUserName(expectedUserName)
                .Build();

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var actual = profileDataRequestContext.IssuedClaims.Select(item => item.Type);
            actual.Should().BeEquivalentTo(expectedClaimTypes);
        }

        [TestCase("someone@email.com", Subject, Email, EmailVerified, PrimaryOrganisationId)]
        [TestCase(null, Subject, PrimaryOrganisationId)]
        public async Task GetProfileDataAsync_GivenApplicationUserWithEmail_ReturnExpectedClaimList(
            string expectedEmail,
            params string[] expectedClaimTypes)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithUserName(null)
                .WithEmail(expectedEmail)
                .Build();

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var actual = profileDataRequestContext.IssuedClaims.Select(item => item.Type);
            actual.Should().BeEquivalentTo(expectedClaimTypes);
        }

        [TestCase("Bob", "Smith", "Bob Smith")]
        [TestCase("   Bob  ", "Smith", "Bob Smith")]
        [TestCase("Bob", "  Smith   ", "Bob Smith")]
        [TestCase("Bob", null, "Bob")]
        [TestCase(null, "Smith", "Smith")]
        public async Task GetProfileDataAsync_GivenAnApplicationUserWithTheName_ReturnsExpectedClaimList(
            string firstname, 
            string lastname, 
            string expectedName)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithUserName(null)
                .WithFirstName(firstname)
                .WithLastName(lastname)
                .Build();

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var expected = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>(Name, expectedName),
            };

            var actual = profileDataRequestContext.IssuedClaims
                .Where(item => Name.Equals(item.Type, StringComparison.Ordinal))
                .Select(item => new KeyValuePair<string, string>(item.Type, item.Value));

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("Authority", Subject, PrimaryOrganisationId, OrganisationFunction, Organisation, Account)]
        [TestCase("Buyer", Subject, PrimaryOrganisationId, OrganisationFunction)]
        public async Task GetProfileDataAsync_GivenApplicationUserWithOrganisationFunction_ReturnExpectedClaimList(
            string organisationFunction, 
            params string[] expectedClaimTypes)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithUserName(null)
                .WithOrganisationFunction(organisationFunction)
                .Build();

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var actual = profileDataRequestContext.IssuedClaims.Select(item => item.Type);
            actual.Should().BeEquivalentTo(expectedClaimTypes);
        }

        [Test]
        public async Task GetProfileDataAsync_GivenNoApplicationUserExists_ReturnsEmptyClaimList()
        {
            const string expectedUserId = "TestUserId";

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedUserId)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var actual = profileDataRequestContext.IssuedClaims.Select(item => new KeyValuePair<string, string>(item.Type, item.Value));
            actual.Should().BeEmpty();
        }

        [Test]
        public async Task GetProfileDataAsync_GivenUserRepositoryDependency_FindByIdAsyncCalledOnce()
        {
            const string expectedUserId = "TestUserId";

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedUserId)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            applicationUserRepositoryMock.Verify(x => x.FindByIdAsync(
                It.Is<string>(param => expectedUserId.Equals(param, StringComparison.Ordinal))), 
                Times.Once);
        }

        [Test]
        public void GetProfileDataAsync_GivenNullSubjectId_ThrowsException()
        {
            static async Task Run()
            {
                var sut = ProfileServiceBuilder
                    .Create()
                    .Build();

                var profileDataRequestContext = ProfileDataRequestContextBuilder
                    .Create()
                    .WithSubjectId(null)
                    .Build();

                await sut.GetProfileDataAsync(profileDataRequestContext);
            }

            Assert.ThrowsAsync<InvalidOperationException>(Run);
        }

        [Test]
        public async Task IsActiveAsync_GivenAnApplicationUser_ShouldReturnTrue()
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .Build();

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var isActiveContext = IsActiveContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.IsActiveAsync(isActiveContext);

            isActiveContext.IsActive.Should().BeTrue();
        }

        [Test]
        public async Task IsActiveAsync_GivenNullApplicationUser_ShouldReturnFalse()
        {
            const string expectedUserId = "TestUserId";

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var isActiveContext = IsActiveContextBuilder
                .Create()
                .WithSubjectId(expectedUserId)
                .Build();

            await sut.IsActiveAsync(isActiveContext);

            isActiveContext.IsActive.Should().BeFalse();
        }

        [Test]
        public async Task IsActiveAsync_GivenUserRepositoryDependency_FindByIdAsyncCalledOnce()
        {
            const string expectedUserId = "TestUserId";

            Mock<IUserRepository> applicationUserRepositoryMock = new Mock<IUserRepository>();
            applicationUserRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .Build();

            var isActiveContext = IsActiveContextBuilder
                .Create()
                .WithSubjectId(expectedUserId)
                .Build();

            await sut.IsActiveAsync(isActiveContext);

            applicationUserRepositoryMock.Verify(x => x.FindByIdAsync(
                    It.Is<string>(param => expectedUserId.Equals(param, StringComparison.Ordinal))), 
                Times.Once);
        }

        [Test]
        public void IsActiveAsync_GivenNullSubjectId_ThrowsException()
        {
            static async Task Run()
            {
                var sut = ProfileServiceBuilder
                    .Create()
                    .Build();

                var isActiveContext = IsActiveContextBuilder
                    .Create()
                    .WithSubjectId(null)
                    .Build();

                await sut.IsActiveAsync(isActiveContext);
            }

            Assert.ThrowsAsync<InvalidOperationException>(Run);
        }
    }
}
