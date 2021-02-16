using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class UsersControllerTests
    {
        [Test]
        public static async Task GetUsersByOrganisationId_ReturnsOkObjectResult()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty);
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<GetAllOrganisationUsersModel>();
        }

        [Test]
        public static async Task GetUsersByOrganisationId_NoUsers_ReturnsEmptyList()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<GetAllOrganisationUsersModel>();
            result.As<OkObjectResult>().Value.As<GetAllOrganisationUsersModel>().Users.Should().BeEmpty();
        }

        [Test]
        public static async Task GetUsersByOrganisationId_ValidId_ReturnsUsers()
        {
            var users = new List<(ApplicationUser RepoUser, OrganisationUserModel Expected)>
            {
                CreateApplicationUserTestData(false),
                CreateApplicationUserTestData(true),
                CreateApplicationUserTestData(false),
            };

            var context = UsersControllerTestContext.Setup();
            context.Users = users.Select(t => t.RepoUser);

            var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty);

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<GetAllOrganisationUsersModel>();
            result.As<OkObjectResult>()
                .Value.As<GetAllOrganisationUsersModel>()
                .Users.Should()
                .BeEquivalentTo(users.Select(t => t.Expected));
        }

        [Test]
        public static async Task GetUsersByOrganisationId_UserRepository_GetUsersByOrganisationIdAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            await controller.GetUsersByOrganisationId(Guid.Empty);

            context.UsersRepositoryMock.Verify(r => r.FindByOrganisationIdAsync(Guid.Empty));
        }

        [Test]
        public static async Task CreateBuyerAsync_CreateBuyerSuccessfulResult_ReturnsUserId()
        {
            const string newUserId = "New Test User Id";

            var context = UsersControllerTestContext.Setup();
            context.CreateBuyerResult = Result.Success(newUserId);

            var controller = context.Controller;

            var response = await controller.CreateBuyerAsync(Guid.Empty, new CreateBuyerRequestModel());

            response.Should().BeOfType<ActionResult<CreateBuyerResponseModel>>();
            var actual = response.Result;

            var expectation = new CreatedAtActionResult(
                nameof(controller.GetUserByIdAsync).TrimAsync(),
                null,
                new { userId = newUserId },
                new CreateBuyerResponseModel { UserId = newUserId });

            actual.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public static async Task CreateBuyerAsync_CreateBuyerService_CreateAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            var organisationId = Guid.NewGuid();
            var createUserRequestViewModel = new CreateBuyerRequestModel
            {
                FirstName = "Bob",
                LastName = "Smith",
                PhoneNumber = "98654321",
                EmailAddress = "bob@smith.com",
            };

            await controller.CreateBuyerAsync(organisationId, createUserRequestViewModel);

            var expected = CreateBuyerRequestBuilder
                .Create()
                .WithFirstName(createUserRequestViewModel.FirstName)
                .WithLastName(createUserRequestViewModel.LastName)
                .WithPhoneNumber(createUserRequestViewModel.PhoneNumber)
                .WithEmailAddress(createUserRequestViewModel.EmailAddress)
                .WithPrimaryOrganisationId(organisationId)
                .Build();

            context.CreateBuyerServiceMock.Verify(s => s.CreateAsync(expected));
        }

        [Test]
        public static async Task CreateBuyerAsync_CreateBuyerFailureResult_ReturnsBadRequest()
        {
            var errors = new List<ErrorDetails> { new("TestErrorId", "TestField") };

            var context = UsersControllerTestContext.Setup();
            context.CreateBuyerResult = Result.Failure<string>(errors);

            var organisationId = Guid.NewGuid();
            var createUserRequestViewModel = new CreateBuyerRequestModel();

            var response = await context.Controller.CreateBuyerAsync(organisationId, createUserRequestViewModel);

            response.Should().BeOfType<ActionResult<CreateBuyerResponseModel>>();
            var actual = response.Result;

            var expectedErrors = new List<ErrorMessageViewModel> { new("TestErrorId", "TestField") };
            var expected = new BadRequestObjectResult(new CreateBuyerResponseModel { Errors = expectedErrors });
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static void CreateBuyerAsync_NullApplicationUser_ThrowsException()
        {
            var context = UsersControllerTestContext.Setup();

            async Task<ActionResult<CreateBuyerResponseModel>> CreateUser()
            {
                var controller = context.Controller;
                return await controller.CreateBuyerAsync(Guid.Empty, null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(CreateUser);
        }

        [Test]
        public static async Task GetUserById_WithExistingUserId_ReturnsTheUser()
        {
            var context = UsersControllerTestContext.Setup();
            context.User = ApplicationUserBuilder.Create().Build();

            var expected = new GetUserModel
            {
                Name = context.User.FirstName + " " + context.User.LastName,
                PhoneNumber = context.User.PhoneNumber,
                EmailAddress = context.User.Email,
                Disabled = context.User.Disabled,
                PrimaryOrganisationId = context.User.PrimaryOrganisationId,
            };

            var controller = context.Controller;

            var result = await controller.GetUserByIdAsync(context.User.Id);

            result.Result.Should().BeOfType<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value.Should().BeOfType<GetUserModel>();
            result.Result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public static async Task GetUserById_NoExistingUserId_ReturnsNotFound()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            var result = await controller.GetUserByIdAsync(string.Empty);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task GetUserById_UserRepository_GetUserByIdAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            await controller.GetUserByIdAsync(string.Empty);

            context.UsersRepositoryMock.Verify(r => r.GetByIdAsync(string.Empty));
        }

        [Test]
        public static async Task EnableUserAsync_GetUserByIdAndEnableThem_ReturnsOk()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(true).Build();

            var result = await controller.EnableUserAsync(context.User.Id);
            result.Should().BeOfType<NoContentResult>();

            context.User.Disabled.Should().BeFalse();
        }

        [Test]
        public static async Task EnableUserAsync_UserIsNull_ReturnsNotFound()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            var result = await controller.EnableUserAsync("unknown");
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task EnableUserAsync_UserRepository_UpdateAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();
            var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(true).Build();

            await controller.EnableUserAsync(context.User.Id);

            context.UsersRepositoryMock.Verify(r => r.GetByIdAsync(context.User.Id));
            context.UsersRepositoryMock.Verify(r => r.UpdateAsync(context.User));
        }

        [Test]
        public static async Task DisableUserAsync_GetUserByIdAndDisableThem_ReturnsOk()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(false).Build();

            var result = await controller.DisableUserAsync(context.User.Id);
            result.Should().BeOfType<NoContentResult>();

            context.User.Disabled.Should().BeTrue();
        }

        [Test]
        public static async Task DisableUserAsync_UserIsNull_ReturnsNotFound()
        {
            var context = UsersControllerTestContext.Setup();

            var controller = context.Controller;

            var result = await controller.DisableUserAsync("unknown");
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public static async Task DisableUser_UserRepository_UpdateAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();
            var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(false).Build();

            await controller.DisableUserAsync(context.User.Id);

            context.UsersRepositoryMock.Verify(r => r.GetByIdAsync(context.User.Id));
            context.UsersRepositoryMock.Verify(r => r.UpdateAsync(context.User));
        }

        private static (ApplicationUser RepoUser, OrganisationUserModel ExpectedUser) CreateApplicationUserTestData(
            bool disabled)
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
                ExpectedUser: new OrganisationUserModel
                {
                    UserId = repositoryApplicationUser.Id,
                    EmailAddress = repositoryApplicationUser.Email,
                    PhoneNumber = repositoryApplicationUser.PhoneNumber,
                    FirstName = repositoryApplicationUser.FirstName,
                    LastName = repositoryApplicationUser.LastName,
                    IsDisabled = repositoryApplicationUser.Disabled,
                });
        }

        private sealed class UsersControllerTestContext
        {
            private UsersControllerTestContext()
            {
                CreateBuyerServiceMock = new Mock<ICreateBuyerService>();
                CreateBuyerServiceMock
                    .Setup(s => s.CreateAsync(It.IsAny<CreateBuyerRequest>()))
                    .ReturnsAsync(() => CreateBuyerResult);

                UsersRepositoryMock = new Mock<IUsersRepository>();

                Users = new List<ApplicationUser>();
                UsersRepositoryMock
                    .Setup(r => r.FindByOrganisationIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(() => Users);

                UsersRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<ApplicationUser>()));

                Controller = new UsersController(CreateBuyerServiceMock.Object, UsersRepositoryMock.Object);

                UsersRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(() => User);

                UsersRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ApplicationUser>()));
            }

            internal Mock<ICreateBuyerService> CreateBuyerServiceMock { get; }

            internal Result<string> CreateBuyerResult { get; set; } = Result.Success("NewUserId");

            internal Mock<IUsersRepository> UsersRepositoryMock { get; }

            internal IEnumerable<ApplicationUser> Users { get; set; }

            internal UsersController Controller { get; }

            internal ApplicationUser User { get; set; }

            internal static UsersControllerTestContext Setup()
            {
                return new();
            }
        }
    }
}
