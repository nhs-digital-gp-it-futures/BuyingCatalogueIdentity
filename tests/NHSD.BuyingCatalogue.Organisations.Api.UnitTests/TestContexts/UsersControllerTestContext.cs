using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts
{
    internal sealed class UsersControllerTestContext
    {
        public Mock<IUsersRepository> UsersRepositoryMock { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public UsersController Controller { get; set; }

        private UsersControllerTestContext()
        {
            UsersRepositoryMock = new Mock<IUsersRepository>();
            
            Users = new List<ApplicationUser>();
            UsersRepositoryMock.Setup(x => x.GetUsersByOrganisationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => Users);

            UsersRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>()));

            Controller = new UsersController(UsersRepositoryMock.Object);
        }

        internal static UsersControllerTestContext Setup()
        {
            return new UsersControllerTestContext();
        }
    }
}
