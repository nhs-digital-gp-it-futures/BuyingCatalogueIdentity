using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CreateOrganisationServiceTests
    {
        [Test]
        public static void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new CreateOrganisationService(null, Mock.Of<IOrganisationValidator>());
            });
        }

        [Test]
        public static void Constructor_NullValidator_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new CreateOrganisationService(Mock.Of<IOrganisationRepository>(), null);
            });
        }

        [Test]
        public static void CreateAsync_NullRequest_Throws()
        {
            var service = new CreateOrganisationService(Mock.Of<IOrganisationRepository>(), Mock.Of<IOrganisationValidator>());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateAsync(null));
        }

        [Test]
        public static async Task CreateAsync_ValidationSuccess_Returns_Success()
        {
            var service = new CreateOrganisationService(SetUpRepository(), SetUpValidator(true));

            var result = await service.CreateAsync(SetUpRequest());

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
        }

        [Test]
        public static async Task CreateAsync_ValidationFailure_Returns_Failure()
        {
            var service = new CreateOrganisationService(SetUpRepository(), SetUpValidator(false));

            var result = await service.CreateAsync(SetUpRequest());

            result.IsSuccess.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Should().BeEquivalentTo(Result.Failure<Guid?>(new List<ErrorDetails>()));
        }

        [Test]
        public static async Task CreateAsync_OnSuccess_Calls_CreateOrganisationAsync_Once()
        {
            var mockOrganisationRepository = SetUpRepositoryMock();
            var service = new CreateOrganisationService(mockOrganisationRepository.Object, SetUpValidator(true));

            await service.CreateAsync(SetUpRequest());

            mockOrganisationRepository.Verify(r => r.CreateOrganisationAsync(It.IsAny<Organisation>()));
        }

        [Test]
        public static async Task CreateAsync_OnFailure_Calls_CreateOrganisationAsync_ZeroTimes()
        {
            var mockOrganisationRepository = SetUpRepositoryMock();
            var service = new CreateOrganisationService(mockOrganisationRepository.Object, SetUpValidator(false));

            await service.CreateAsync(SetUpRequest());

            mockOrganisationRepository.Verify(r => r.CreateOrganisationAsync(It.IsAny<Organisation>()), Times.Never());
        }

        [Test]
        public static async Task CreateAsync_CreatesOrganisationWithCorrectValues()
        {
            var expected = OrganisationBuilder.Create(1).WithAddress(AddressBuilder.Create().Build()).Build();

            var calledBack = false;
            var mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository.Setup(r => r.CreateOrganisationAsync(It.IsAny<Organisation>()))
                .Callback((Organisation actual) =>
                {
                    actual.Should().BeEquivalentTo(expected, c => c
                        .Excluding(o => o.OrganisationId)
                        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1000))
                        .WhenTypeIs<DateTime>());
                    actual.OrganisationId.Should().NotBeEmpty();
                    calledBack = true;
                });

            var service = new CreateOrganisationService(mockOrganisationRepository.Object, SetUpValidator(true));

            await service.CreateAsync(SetUpRequest(
                expected.Name,
                expected.OdsCode,
                expected.PrimaryRoleId,
                expected.CatalogueAgreementSigned,
                expected.Address));

            calledBack.Should().BeTrue();
        }

        private static IOrganisationRepository SetUpRepository()
        {
            return SetUpRepositoryMock().Object;
        }

        private static Mock<IOrganisationRepository> SetUpRepositoryMock()
        {
            var mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository.Setup(r => r.CreateOrganisationAsync(It.IsAny<Organisation>()));
            return mockOrganisationRepository;
        }

        private static IOrganisationValidator SetUpValidator(bool returnsSuccess)
        {
            var mockValidator = new Mock<IOrganisationValidator>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Organisation>()))
                .ReturnsAsync(returnsSuccess ? Result.Success() : Result.Failure());
            return mockValidator.Object;
        }

        private static CreateOrganisationRequest SetUpRequest(
            string name = null,
            string odsCode = null,
            string primaryRoleId = null,
            bool catalogueAgreementSigned = false,
            Address address = null)
        {
            return new(name, odsCode, primaryRoleId, catalogueAgreementSigned, address);
        }
    }
}
