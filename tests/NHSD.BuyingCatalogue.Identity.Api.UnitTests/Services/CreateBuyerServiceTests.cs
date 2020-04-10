using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Comparers;
using NHSD.BuyingCatalogue.Identity.Api.Validators;
using NHSD.BuyingCatalogue.Identity.Common.Messages;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class CreateBuyerServiceTests
    {
        [Test]
        public void Constructor_NullApplicationUserValidator_ThrowsException()
        {
            static void Test()
            {
                var context = CreateBuyerServiceTestContext.Setup();
                var sut = new CreateBuyerService(null, context.UsersRepositoryMock.Object, context.RegistrationServiceMock.Object);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullUserRepository_ThrowsException()
        {
            static void Test()
            {
                var context = CreateBuyerServiceTestContext.Setup();
                var sut = new CreateBuyerService(context.ApplicationUserValidatorMock.Object, null, context.RegistrationServiceMock.Object);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Constructor_NullRegistrationService_ThrowsException()
        {
            static void Test()
            {
                var context = CreateBuyerServiceTestContext.Setup();
                var sut = new CreateBuyerService(context.ApplicationUserValidatorMock.Object, context.UsersRepositoryMock.Object, null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void CreateAsync_NullCreateBuyerRequest_ThrowsException()
        {
            static async Task TestAsync()
            {
                var context = CreateBuyerServiceTestContext.Setup();
                var sut = context.CreateBuyerService;

                _ = await sut.CreateAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        }

        [Test]
        public async Task CreateAsync_SuccessfulApplicationUserValidation_ReturnsSuccess()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            var sut = context.CreateBuyerService;

            var request = CreateBuyerRequestBuilder.Create().Build();

            var actual = await sut.CreateAsync(request);

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull();
        }

        [Test]
        public async Task CreateAsync_ApplicationUserValidation_CalledOnce()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            var sut = context.CreateBuyerService;

            var request = CreateBuyerRequestBuilder.Create().Build();

            await sut.CreateAsync(request);

            var expected = ApplicationUserBuilder
                .Create()
                .WithFirstName(request.FirstName)
                .WithLastName(request.LastName)
                .WithPhoneNumber(request.PhoneNumber)
                .WithEmailAddress(request.EmailAddress)
                .WithPrimaryOrganisationId(request.PrimaryOrganisationId)
                .Build();

            context.ApplicationUserValidatorMock.Verify(x => 
                x.ValidateAsync(It.Is<ApplicationUser>(
                    actual => ApplicationUserEditableInformationComparer.Instance.Equals(expected, actual))), Times.Once);
        }

        [Test]
        public async Task CreateAsync_SuccessfulApplicationUserValidation_UserRepository_CalledOnce()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            var sut = context.CreateBuyerService;

            var request = CreateBuyerRequestBuilder.Create().Build();

            await sut.CreateAsync(request);

            var expected = ApplicationUserBuilder
                .Create()
                .WithFirstName(request.FirstName)
                .WithLastName(request.LastName)
                .WithPhoneNumber(request.PhoneNumber)
                .WithEmailAddress(request.EmailAddress)
                .WithPrimaryOrganisationId(request.PrimaryOrganisationId)
                .Build();

            context.UsersRepositoryMock.Verify(x => 
                x.CreateUserAsync(It.Is<ApplicationUser>(
                    actual => ApplicationUserEditableInformationComparer.Instance.Equals(expected, actual))), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ApplicationUserValidationFails_ReturnFailureResult()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            context.ApplicationUserValidatorResult = Result.Failure(new List<ErrorMessage>());

            var sut = context.CreateBuyerService;

            var request = CreateBuyerRequestBuilder.Create().Build();

            var actual = await sut.CreateAsync(request);

            var expected = Result.Failure(new List<ErrorMessage>());
            actual.Should().Be(expected);
        }

        [Test]
        public async Task CreateBuyerAsync_NewApplicationUser_SendsEmail()
        {
            var context = CreateBuyerServiceTestContext.Setup();

            var sut = context.CreateBuyerService;

            var request = CreateBuyerRequestBuilder.Create().Build();

            await sut.CreateAsync(request);

            var expected = ApplicationUserBuilder
                .Create()
                .WithFirstName(request.FirstName)
                .WithLastName(request.LastName)
                .WithPhoneNumber(request.PhoneNumber)
                .WithEmailAddress(request.EmailAddress)
                .WithPrimaryOrganisationId(request.PrimaryOrganisationId)
                .Build();

            context.RegistrationServiceMock.Verify(x => 
                x.SendInitialEmailAsync(It.Is<ApplicationUser>(
                    actual => ApplicationUserEditableInformationComparer.Instance.Equals(expected, actual))), Times.Once);
        }
    }

    internal sealed class CreateBuyerServiceTestContext
    {
        private CreateBuyerServiceTestContext()
        {
            ApplicationUserValidatorMock = new Mock<IApplicationUserValidator>();
            ApplicationUserValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(() => ApplicationUserValidatorResult);

            UsersRepositoryMock = new Mock<IUsersRepository>();
            UsersRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>()));

            RegistrationServiceMock = new Mock<IRegistrationService>();

            CreateBuyerService = new CreateBuyerService(ApplicationUserValidatorMock.Object, UsersRepositoryMock.Object, RegistrationServiceMock.Object);
        }

        internal Mock<IApplicationUserValidator> ApplicationUserValidatorMock { get; set; }

        internal Result ApplicationUserValidatorResult { get; set; } = Result.Success();

        internal Mock<IUsersRepository> UsersRepositoryMock { get; set; }

        internal CreateBuyerService CreateBuyerService { get; }

        internal Mock<IRegistrationService> RegistrationServiceMock { get; set; }

        public static CreateBuyerServiceTestContext Setup()
        {
            return new CreateBuyerServiceTestContext();
        }
    }
}
