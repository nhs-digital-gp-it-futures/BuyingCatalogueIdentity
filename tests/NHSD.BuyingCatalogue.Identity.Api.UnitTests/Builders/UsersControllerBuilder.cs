using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class UsersControllerBuilder
    {
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private IEnumerable<ApplicationUser> _users;

        internal UsersControllerBuilder()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            
            _users = new List<ApplicationUser>();
            _usersRepositoryMock.Setup(x => x.GetUsersByOrganisationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => _users);
        }

        internal UsersControllerBuilder SetUsers(IEnumerable<ApplicationUser> users)
        {
            _users = users;
            return this;
        }

        internal UsersController Build()
        {
            return new UsersController(_usersRepositoryMock.Object);
        }
    }
}
