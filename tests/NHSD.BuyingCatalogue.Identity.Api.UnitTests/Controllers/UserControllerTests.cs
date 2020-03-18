using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class UserControllerTests
    {
        [Test]
        public async Task GetUsersByOrganisationId_NoOrganisation_Returns404()
        {
            using var controller = new UsersControllerBuilder()
                .SetOrganisation(null)
                .Build();

            var result = await controller.GetUsersByOrganisationId(Guid.Empty);

            result.Should().BeOfType<NotFoundResult>();
        }

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
            var repoUsers = new List<ApplicationUser> { 
                CreateUser(false),
                CreateUser(true),
                CreateUser(false)
                }.OrderBy(x => x.Id).ToList();

            using var controller = new UsersControllerBuilder()
                .SetUsers(repoUsers)
                .Build();

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            
            result.Should().NotBeNull();
            var users = result.Value as GetAllOrganisationUsersViewModel;
            users.Should().NotBeNull();

            var usersSorted = users.Users.OrderBy(x => x.UserId).ToList();
            AssertEqualUsers(usersSorted, repoUsers);
        }

        internal static ApplicationUser CreateUser(bool disabled)
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Disabled = disabled
            };
        }

        internal static void AssertEqualUsers(IEnumerable<OrganisationUserViewModel> actuals, IEnumerable<ApplicationUser> expecteds)
        {
            actuals.Count().Should().Be(expecteds.Count());

            for (var i = 0; i < expecteds.Count(); i++)
            {
                var actual = actuals.ElementAt(i);
                var expected = expecteds.ElementAt(i);

                actual.PhoneNumber.Should().Be(expected.PhoneNumber);
                actual.EmailAddress.Should().Be(expected.Email);
                actual.UserId.Should().Be(expected.Id);
                actual.IsDisabled.Should().Be(expected.Disabled);
                actual.FirstName.Should().Be(expected.FirstName);
                actual.LastName.Should().Be(expected.LastName);
            }
        }
    }
}
