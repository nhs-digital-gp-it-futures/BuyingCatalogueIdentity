using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Comparers;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class UsersControllerTests
    {
        [Test]
        public async Task GetUsersByOrganisationId_ReturnsOkObjectResult()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty);
            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult).Value.Should().BeOfType<GetAllOrganisationUsersViewModel>();
        }

        [Test]
        public async Task GetUsersByOrganisationId_NoUsers_ReturnsEmptyList()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            var users = result.Value as GetAllOrganisationUsersViewModel;
            users.Users.Should().BeEmpty();
        }

        [Test]
        public async Task GetUsersByOrganisationId_ValidId_ReturnsUsers()
        {
            var users = new List<(ApplicationUser RepoUser, OrganisationUserViewModel Expected)>
            {
                CreateApplicationUserTestData(false),
                CreateApplicationUserTestData(true),
                CreateApplicationUserTestData(false)
            };

            var context = UsersControllerTestContext.Setup();
            context.Users = users.Select(x => x.RepoUser);

            using var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            var viewModel = result.Value as GetAllOrganisationUsersViewModel;

            viewModel.Users.Should().BeEquivalentTo(users.Select(x => x.Expected));
        }

        [Test]
        public async Task GetUsersByOrganisationId_UserRepository_GetUsersByOrganisationIdAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            await controller.GetUsersByOrganisationId(Guid.Empty);

            context.UsersRepositoryMock.Verify(x => x.GetUsersByOrganisationIdAsync(Guid.Empty), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_NewApplicationUser_ReturnsStatusOk()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var response = await controller.CreateUserAsync(Guid.Empty, new CreateUserRequestViewModel());

            response.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task CreateUserAsync_UserRepository_CreateUserAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var organisationId = Guid.NewGuid();
            var createUserRequestViewModel = new CreateUserRequestViewModel
            {
                FirstName = "Bob",
                LastName = "Smith",
                PhoneNumber = "0123456789",
                EmailAddress = "a.b@c.com"
            };

            await controller.CreateUserAsync(organisationId, createUserRequestViewModel);

            var expectedApplicationUser = ApplicationUserBuilder
                .Create()
                .WithFirstName(createUserRequestViewModel.FirstName)
                .WithLastName(createUserRequestViewModel.LastName)
                .WithPhoneNumber(createUserRequestViewModel.PhoneNumber)
                .WithEmailAddress(createUserRequestViewModel.EmailAddress)
                .WithPrimaryOrganisationId(organisationId)
                .Build();

            context.UsersRepositoryMock.Verify(x => x.CreateUserAsync(
                It.Is<ApplicationUser>(
                    actual => ApplicationUserEditableInformationComparer.Instance.Equals(expectedApplicationUser, actual))),
                Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_NewApplicationUser_SendsEmail()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            await controller.CreateUserAsync(Guid.Empty, new CreateUserRequestViewModel());

            context.RegistrationServiceMock.Verify(r => r.SendInitialEmailAsync(It.IsNotNull<ApplicationUser>()), Times.Once());
        }

        [Test]
        public void CreateUserAsync_NullApplicationUser_ThrowsException()
        {
            var context = UsersControllerTestContext.Setup();

            async Task<ActionResult> CreateUser()
            {
                using var controller = context.Controller;
                return await controller.CreateUserAsync(Guid.Empty, null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(CreateUser);
        }

        private static (ApplicationUser RepoUser, OrganisationUserViewModel ExpectedUser) CreateApplicationUserTestData(bool disabled)
        {
            var repositoryApplicationUser = ApplicationUserBuilder
                .Create()
                .WithFirstName(Guid.NewGuid().ToString())
                .WithLastName(Guid.NewGuid().ToString())
                .WithPhoneNumber(Guid.NewGuid().ToString())
                .WithEmailAddress(Guid.NewGuid().ToString())
                .WithDisabled(disabled)
                .Build();

            return (
                RepoUser: repositoryApplicationUser,
                ExpectedUser: new OrganisationUserViewModel
                {
                    UserId = repositoryApplicationUser.Id,
                    EmailAddress = repositoryApplicationUser.Email,
                    PhoneNumber = repositoryApplicationUser.PhoneNumber,
                    FirstName = repositoryApplicationUser.FirstName,
                    LastName = repositoryApplicationUser.LastName,
                    IsDisabled = repositoryApplicationUser.Disabled
                }
            );
        }
    }
}
