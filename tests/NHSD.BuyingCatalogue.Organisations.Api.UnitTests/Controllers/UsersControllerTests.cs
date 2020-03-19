using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class UsersControllerTests
    {
        [Test]
        public async Task GetUsersByOrganisationId_NoUsers_ReturnsEmptyList()
        {
            var built = new UsersControllerBuilder().Build();
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
                CreateUser(false), CreateUser(true), CreateUser(false)
            };
            var built = new UsersControllerBuilder()
                .SetUsers(users.Select(x => x.RepoUser))
                .Build();
            using var controller = built.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            
            result.Should().NotBeNull();
            var viewModel = result.Value as GetAllOrganisationUsersViewModel;
            viewModel.Should().NotBeNull();
            
            viewModel.Users.Should().BeEquivalentTo(users.Select(x => x.Expected));

            built.UserRepository.Verify(x => x.GetUsersByOrganisationIdAsync(Guid.Empty), Times.Once);
        }

        internal static (ApplicationUser RepoUser, OrganisationUserViewModel ExpectedUser) CreateUser(bool disabled)
        {
            var repoUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Disabled = disabled
            };
            return (
                RepoUser: repoUser,
                ExpectedUser: new OrganisationUserViewModel
                {
                    UserId = repoUser.Id,
                    EmailAddress = repoUser.Email,
                    PhoneNumber = repoUser.PhoneNumber,
                    FirstName = repoUser.FirstName,
                    LastName = repoUser.LastName,
                    IsDisabled = repoUser.Disabled
                }
                );
        }
    }
}
