using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class UsersControllerBuilder
    {
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private IEnumerable<ApplicationUser> _users;

        private UsersControllerBuilder()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            
            _users = new List<ApplicationUser>();
            _usersRepositoryMock.Setup(x => x.GetUsersByOrganisationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => _users);

            _usersRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>()));
        }

        internal static UsersControllerBuilder Create()
        {
            return new UsersControllerBuilder();
        }

        internal UsersControllerBuilder SetUsers(IEnumerable<ApplicationUser> users)
        {
            _users = users;
            return this;
        }

        internal (UsersController Controller, Mock<IUsersRepository> UserRepository) Build()
        {
            return (new UsersController(_usersRepositoryMock.Object), _usersRepositoryMock);
        }
    }
}
