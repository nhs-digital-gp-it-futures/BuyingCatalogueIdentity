using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    internal sealed class UsersControllerBuilder
    {
        private readonly Mock<IOrganisationRepository> _organisationMock;
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private Organisation _organisation;
        private IEnumerable<ApplicationUser> _users;

        internal UsersControllerBuilder()
        {
            _organisationMock = new Mock<IOrganisationRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _organisation = new Organisation();
            _organisationMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => _organisation);

            _users = new List<ApplicationUser>();
            _usersRepositoryMock.Setup(x => x.GetUsersByOrganisationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => _users);
        }

        internal UsersControllerBuilder SetUsers(IEnumerable<ApplicationUser> users)
        {
            _users = users;
            return this;
        }

        internal UsersControllerBuilder SetOrganisation(Organisation organisation)
        {
            _organisation = organisation;
            return this;
        }

        internal UsersController Build()
        {
            return new UsersController(_usersRepositoryMock.Object, _organisationMock.Object);
        }
    }
}
