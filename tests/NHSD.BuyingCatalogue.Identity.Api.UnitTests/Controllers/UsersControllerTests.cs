using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class UsersControllerTests
    {
        [Test]
        public async Task GetUsersByOrganisationId_NoUsers_ReturnsEmptyList()
        {
            using var controller = new UsersControllerBuilder().Build();

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;

            result.Should().NotBeNull();
            var users = result.Value as GetAllOrganisationUsersViewModel;
            users.Should().NotBeNull();
            users.Users.Should().BeEmpty();
        }

        [Test]
        public async Task GetUsersByOrganisationId_ValidId_ReturnsUsers()
        {
            var users = new List<(ApplicationUser RepoUser, OrganisationUserViewModel Expected)>
            {
                CreateUser(false), CreateUser(true), CreateUser(false)
            };

            using var controller = new UsersControllerBuilder()
                .SetUsers(users.Select(x => x.RepoUser))
                .Build();

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            
            result.Should().NotBeNull();
            var viewModel = result.Value as GetAllOrganisationUsersViewModel;
            viewModel.Should().NotBeNull();
            
            viewModel.Users.Should().BeEquivalentTo(users.Select(x => x.Expected));
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
