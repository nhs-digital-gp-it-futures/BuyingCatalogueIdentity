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
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NUnit.Framework;
using static IdentityModel.JwtClaimTypes;
using static NHSD.BuyingCatalogue.Identity.Common.Constants.ApplicationPermissions;
using OrganisationFunction = NHSD.BuyingCatalogue.Identity.Api.Models.OrganisationFunction;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class ProfileServiceTests
    {
        [Test]
        public async Task GetProfileDataAsync_GivenAnApplicationUserExistsWithOrganisationFunctionAuthority_ReturnsExpectedClaimList()
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithEmailAddress("TestUser@Email.com")
                .WithFirstName("Bob")
                .WithLastName("Smith")
                .WithOrganisationFunction(OrganisationFunction.Authority)
                .Build();

            var expectedOrganisation = OrganisationBuilder
                .Create()
                .WithName("Primary Health Trust")
                .Build();

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            Mock<IOrganisationRepository> organisationRespositoryMock = new Mock<IOrganisationRepository>();
            organisationRespositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedOrganisation);

            var sut = ProfileServiceBuilder
                    .Create()
                    .WithUserRepository(applicationUserRepositoryMock.Object)
                    .WithOrganisationRepository(organisationRespositoryMock.Object)
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
                (ApplicationClaimTypes.PrimaryOrganisationId, expectedApplicationUser.PrimaryOrganisationId.ToString()),
                (ApplicationClaimTypes.PrimaryOrganisationName,expectedOrganisation.Name),
                (ApplicationClaimTypes.OrganisationFunction, expectedApplicationUser.OrganisationFunction.DisplayName),
                (ApplicationClaimTypes.Organisation, Manage),
                (ApplicationClaimTypes.Account, Manage)
            };

            var actual = profileDataRequestContext.IssuedClaims.Select(item => (item.Type, item.Value));
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task
            GetProfileDataAsync_GivenAnApplicationUserExistsWithOrganisationFunctionBuyer_ReturnsExpectedClaimList()
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithEmailAddress("TestUser@Email.com")
                .WithFirstName("Bob")
                .WithLastName("Smith")
                .WithOrganisationFunction(OrganisationFunction.Buyer)
                .Build();

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var expectedOrganisation = OrganisationBuilder
                .Create()
                .WithName("Primary Health Trust")
                .Build();

            Mock<IOrganisationRepository> organisationRespositoryMock = new Mock<IOrganisationRepository>();
            organisationRespositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedOrganisation);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .WithOrganisationRepository(organisationRespositoryMock.Object)
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
                (EmailVerified,
                    expectedApplicationUser.EmailConfirmed.ToString(CultureInfo.CurrentCulture).ToLowerInvariant()),
                (ApplicationClaimTypes.PrimaryOrganisationId,
                    expectedApplicationUser.PrimaryOrganisationId.ToString()),
                (ApplicationClaimTypes.PrimaryOrganisationName,
                    expectedOrganisation.Name),
                (ApplicationClaimTypes.OrganisationFunction,
                    expectedApplicationUser.OrganisationFunction.DisplayName),
                (ApplicationClaimTypes.Ordering, Manage)
            };

            var actual = profileDataRequestContext.IssuedClaims.Select(item => (item.Type, item.Value));
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("SomeId", "SomeUserName", Subject, PreferredUserName, JwtRegisteredClaimNames.UniqueName, ApplicationClaimTypes.PrimaryOrganisationId, ApplicationClaimTypes.PrimaryOrganisationName, ApplicationClaimTypes.OrganisationFunction, ApplicationClaimTypes.Ordering)]
        [TestCase("SomeId", "", Subject, ApplicationClaimTypes.PrimaryOrganisationId, ApplicationClaimTypes.PrimaryOrganisationName, ApplicationClaimTypes.OrganisationFunction, ApplicationClaimTypes.Ordering)]
        public async Task GetProfileDataAsync_GivenApplicationUserWithId_ReturnExpectedClaimList(
            string expectedUserId,
            string expectedUserName,
            params string[] expectedClaimTypes)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithUserId(expectedUserId)
                .WithUsername(expectedUserName)
                .WithFirstName(string.Empty)
                .WithLastName(string.Empty)
                .WithOrganisationFunction(OrganisationFunction.Buyer)
                .WithEmailAddress(string.Empty)
                .Build();

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var expectedOrganisation = OrganisationBuilder
                .Create()
                .WithName("Primary Health Trust")
                .Build();

            Mock<IOrganisationRepository> organisationRespositoryMock = new Mock<IOrganisationRepository>();
            organisationRespositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedOrganisation);

            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .WithOrganisationRepository(organisationRespositoryMock.Object)
                .Build();

            var profileDataRequestContext = ProfileDataRequestContextBuilder
                .Create()
                .WithSubjectId(expectedApplicationUser.Id)
                .Build();

            await sut.GetProfileDataAsync(profileDataRequestContext);

            var actual = profileDataRequestContext.IssuedClaims.Select(item => item.Type);
            actual.Should().BeEquivalentTo(expectedClaimTypes);
        }

        [TestCase("someone@email.com", Subject, Email, EmailVerified, ApplicationClaimTypes.PrimaryOrganisationId, ApplicationClaimTypes.PrimaryOrganisationName, ApplicationClaimTypes.OrganisationFunction, ApplicationClaimTypes.Ordering)]
        [TestCase("", Subject, ApplicationClaimTypes.PrimaryOrganisationId, ApplicationClaimTypes.PrimaryOrganisationName, ApplicationClaimTypes.OrganisationFunction, ApplicationClaimTypes.Ordering)]
        public async Task GetProfileDataAsync_GivenApplicationUserWithEmail_ReturnExpectedClaimList(
            string expectedEmail,
            params string[] expectedClaimTypes)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithUsername(string.Empty)
                .WithFirstName(string.Empty)
                .WithLastName(string.Empty)
                .WithOrganisationFunction(OrganisationFunction.Buyer)
                .WithEmailAddress(expectedEmail)
                .Build();

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var expectedOrganisation = OrganisationBuilder
                .Create()
                .WithName("Primary Health Trust")
                .Build();

            Mock<IOrganisationRepository> organisationRespositoryMock = new Mock<IOrganisationRepository>();
            organisationRespositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedOrganisation);


            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .WithOrganisationRepository(organisationRespositoryMock.Object)
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
        [TestCase("Bob", "", "Bob")]
        [TestCase("", "Smith", "Smith")]
        public async Task GetProfileDataAsync_GivenAnApplicationUserWithTheName_ReturnsExpectedClaimList(
            string firstname, 
            string lastname, 
            string expectedName)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithFirstName(firstname)
                .WithLastName(lastname)
                .Build();

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var expectedOrganisation = OrganisationBuilder
                .Create()
                .WithName("Primary Health Trust")
                .Build();

            Mock<IOrganisationRepository> organisationRespositoryMock = new Mock<IOrganisationRepository>();
            organisationRespositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedOrganisation);


            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .WithOrganisationRepository(organisationRespositoryMock.Object)
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

        [TestCase("Authority", Subject, ApplicationClaimTypes.PrimaryOrganisationId, ApplicationClaimTypes.PrimaryOrganisationName, ApplicationClaimTypes.OrganisationFunction, ApplicationClaimTypes.Organisation, ApplicationClaimTypes.Account)]
        [TestCase("Buyer", Subject, ApplicationClaimTypes.PrimaryOrganisationId, ApplicationClaimTypes.PrimaryOrganisationName, ApplicationClaimTypes.OrganisationFunction, ApplicationClaimTypes.Ordering)]
        public async Task GetProfileDataAsync_GivenApplicationUserWithOrganisationFunction_ReturnExpectedClaimList(
            string organisationFunctionDisplayName, 
            params string[] expectedClaimTypes)
        {
            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithUsername(string.Empty)
                .WithFirstName(string.Empty)
                .WithLastName(string.Empty)
                .WithEmailAddress(string.Empty)
                .WithOrganisationFunction(OrganisationFunction.FromDisplayName(organisationFunctionDisplayName))
                .Build();

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplicationUser);

            var expectedOrganisation = OrganisationBuilder
                .Create()
                .WithName("Primary Health Trust")
                .Build();

            Mock<IOrganisationRepository> organisationRespositoryMock = new Mock<IOrganisationRepository>();
            organisationRespositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedOrganisation);


            var sut = ProfileServiceBuilder
                .Create()
                .WithUserRepository(applicationUserRepositoryMock.Object)
                .WithOrganisationRepository(organisationRespositoryMock.Object)
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

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
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

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
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

            applicationUserRepositoryMock.Verify(x => x.GetByIdAsync(
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

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
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

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
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

            Mock<IUsersRepository> applicationUserRepositoryMock = new Mock<IUsersRepository>();
            applicationUserRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
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

            applicationUserRepositoryMock.Verify(x => x.GetByIdAsync(
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
