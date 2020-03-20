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
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class UsersControllerTests
    {
        [Test]
        public async Task GetUsersByOrganisationId_NoUsers_ReturnsEmptyList()
        {
            var built = UsersControllerBuilder
                .Create()
                .Build();

            using var controller = built.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;

            result.Should().NotBeNull();
            var users = result.Value as GetAllOrganisationUsersViewModel;
            users.Should().NotBeNull();
            users.Users.Should().BeEmpty();

            built.UserRepository.Verify(x => x.GetUsersByOrganisationIdAsync(Guid.Empty), Times.Once);
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

            var built = UsersControllerBuilder
                .Create()
                .SetUsers(users.Select(x => x.RepoUser))
                .Build();

            using var controller = built.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;

            result.Should().NotBeNull();
            var viewModel = result.Value as GetAllOrganisationUsersViewModel;
            viewModel.Should().NotBeNull();

            viewModel.Users.Should().BeEquivalentTo(users.Select(x => x.Expected));
        }

        [Test]
        public async Task GetUsersByOrganisationId_UserRepository_GetUsersByOrganisationIdAsync_CalledOnce()
        {
            var built = UsersControllerBuilder
                .Create()
                .Build();

            using var controller = built.Controller;

            await controller.GetUsersByOrganisationId(Guid.Empty);

            built.UserRepository.Verify(x => x.GetUsersByOrganisationIdAsync(Guid.Empty), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_NewApplicationUser_ReturnsStatusOk()
        {
            var built = UsersControllerBuilder
                .Create()
                .Build();

            using var controller = built.Controller;

            var response = await controller.CreateUserAsync(Guid.Empty, new CreateUserRequestViewModel()) as OkResult;

            response.Should().BeEquivalentTo(new OkResult());
        }

        [Test]
        public async Task CreateUserAsync_UserRepository_CreateUserAsync_CalledOnce()
        {
            var built = UsersControllerBuilder
                .Create()
                .Build();

            using var controller = built.Controller;

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

            built.UserRepository.Verify(x => x.CreateUserAsync(
                It.Is<ApplicationUser>(
                    actual => ApplicationUserEditableInformationComparer.Instance.Equals(expectedApplicationUser, actual)))
                , Times.Once);
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
