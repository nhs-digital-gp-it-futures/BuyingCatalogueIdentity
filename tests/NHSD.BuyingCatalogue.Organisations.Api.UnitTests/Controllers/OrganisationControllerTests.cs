using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.ViewModels.Messages;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OrganisationControllerTests
    {
        private readonly Address _address1 = AddressBuilder.Create().WithLine1("18 Stone Road").Build();

        [Test]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrganisationsController(null, Mock.Of<ICreateOrganisationService>());
            });
        }

        [Test]
        public void Constructor_NullService_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new OrganisationsController(Mock.Of<IOrganisationRepository>(), null);
            });
        }

        [Test]
        public async Task GetAllAsync_NoOrganisationsExist_EmptyResultIsReturned()
        {
            using var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>())
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<GetAllOrganisationsViewModel>();

            var organisationResult = objectResult.Value as GetAllOrganisationsViewModel;

            organisationResult.Organisations.Count().Should().Be(0);
        }

        [TestCase(null, null, null, false, false)]
        [TestCase("Organisation One", null, null, false, false)]
        [TestCase(null, "ODS 1", null, false, false)]
        [TestCase("Organisation One", "ODS 1", null, false, false)]
        [TestCase("Organisation One", "ODS 1", null, true, false)]
        [TestCase("Organisation One", "ODS 1", null, true, true)]
        public async Task GetAllAsync_SingleOrganisationExists_ReturnsTheOrganisation(string name, string ods, string primaryRoleId, bool catalogueAgreementSigned, bool hasAddress)
        {
            using var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>()
                {
                    OrganisationBuilder.Create(1)
                        .WithName(name)
                        .WithOdsCode(ods)
                        .WithPrimaryRoleId(primaryRoleId)
                        .WithCatalogueAgreementSigned(catalogueAgreementSigned)
                        .WithAddress(hasAddress == false ? null : _address1)
                        .Build()
                })
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<GetAllOrganisationsViewModel>();

            var organisationResult = objectResult.Value as GetAllOrganisationsViewModel;

            organisationResult.Organisations.Count().Should().Be(1);

            var organisationList = organisationResult.Organisations.ToList();
            organisationList[0].Name.Should().Be(name);
            organisationList[0].OdsCode.Should().Be(ods);
            organisationList[0].PrimaryRoleId.Should().Be(primaryRoleId);
            organisationList[0].CatalogueAgreementSigned.Should().Be(catalogueAgreementSigned);

            if (hasAddress)
            {
                organisationList[0].Address.Should().BeEquivalentTo(_address1);
            }
            else
            {
                organisationList[0].Address.Should().BeNull();
            }
        }

        [Test]
        public async Task GetAllAsync_ListOfOrganisationsExist_ReturnsTheOrganisations()
        {
            Address address = AddressBuilder.Create().WithLine1("2 City Close").Build();

            var org1 = OrganisationBuilder.Create(1).WithCatalogueAgreementSigned(false).WithAddress(_address1).Build();

            var org2 = OrganisationBuilder.Create(2).WithAddress(address).Build();

            var org3 = OrganisationBuilder.Create(3).Build();

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>() { org1, org2, org3 })
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<GetAllOrganisationsViewModel>();

            var organisationResult = objectResult.Value as GetAllOrganisationsViewModel;

            organisationResult.Organisations.Count().Should().Be(3);

            var organisationList = organisationResult.Organisations.ToList();

            organisationList[0].Should().BeEquivalentTo(org1, config => config.Excluding(x => x.LastUpdated));
            organisationList[1].Should().BeEquivalentTo(org2, config => config.Excluding(x => x.LastUpdated));
            organisationList[2].Should().BeEquivalentTo(org3, config => config.Excluding(x => x.LastUpdated));
        }

        [Test]
        public async Task GetAllAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            var getAllOrganisations = new Mock<IOrganisationRepository>();
            getAllOrganisations.Setup(x => x.ListOrganisationsAsync())
                .ReturnsAsync(new List<Organisation>());

            using var controller = OrganisationControllerBuilder.Create()
                .WithOrganisationRepository(getAllOrganisations.Object)
                .Build();

            await controller.GetAllAsync();

            getAllOrganisations.Verify(x => x.ListOrganisationsAsync(), Times.Once);
        }

        [Test]
        public async Task GetIdByAsync_OrganisationDoesNotExist_ReturnsNotFound()
        {
            using var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(null)
                .Build();

            var result = await controller.GetByIdAsync(Guid.NewGuid());

            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task GetIdByAsync_OrganisationExists_ReturnsTheOrganisation()
        {
            var organisation = OrganisationBuilder.Create(1).WithAddress(_address1).Build();

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.OrganisationId);

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;

            objectResult.Value.Should().BeEquivalentTo(organisation,
                conf => conf.Excluding(c => c.LastUpdated));
        }

        [Test]
        public async Task GetByIdAsync_OrganisationAddressIsNull_ReturnsOrganisationWithNullAddress()
        {
            var organisation = OrganisationBuilder.Create(1).Build();

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.OrganisationId);

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;

            objectResult.Value.Should().BeEquivalentTo(organisation,
                conf => conf.Excluding(c => c.LastUpdated));
        }

        [Test]
        public async Task GetIdByAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            Guid expectedId = Guid.NewGuid();

            var mockGetOrganisation = new Mock<IOrganisationRepository>();
            mockGetOrganisation.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Organisation);

            using var controller = OrganisationControllerBuilder.Create()
                .WithOrganisationRepository(mockGetOrganisation.Object)
                .Build();

            await controller.GetByIdAsync(expectedId);

            mockGetOrganisation.Verify(x => x.GetByIdAsync(expectedId), Times.Once);
        }

        [Test]
        public async Task UpdateOrganisationByIdAsync_UpdateOrganisation_ReturnsStatusNoContent()
        {
            using var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(new Organisation()).Build();

            var response = await controller.UpdateOrganisationByIdAsync(Guid.Empty, new UpdateOrganisationViewModel());

            response.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateOrganisationByIdAsync_UpdateOrganisation_NonExistentOrganisation_ReturnsStatusNotFound()
        {
            using var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(null).Build();

            var response = await controller.UpdateOrganisationByIdAsync(Guid.Empty, new UpdateOrganisationViewModel());

            response.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task UpdateOrganisationByIdAsync_UpdateOrganisation_UpdatesCatalogueAgreementSigned()
        {
            var organisation = OrganisationBuilder.Create(1).WithCatalogueAgreementSigned(true).Build();

            using var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(organisation).Build();

            var response = await controller.UpdateOrganisationByIdAsync(organisation.OrganisationId, new UpdateOrganisationViewModel { CatalogueAgreementSigned = false });

            response.Should().BeOfType<NoContentResult>();

            organisation.CatalogueAgreementSigned.Should().BeFalse();
        }

        [Test]
        public async Task UpdateOrganisationByIdAsync_OrganisationRepository_UpdateAsync_And_GetByIdAsync_CalledOnce()
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Organisation());
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Organisation>()));

            using var controller = OrganisationControllerBuilder.Create().WithOrganisationRepository(repositoryMock.Object).Build();

            await controller.UpdateOrganisationByIdAsync(Guid.Empty, new UpdateOrganisationViewModel());

            repositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<Organisation>()), Times.Once);

            repositoryMock.Verify(x => x.GetByIdAsync(
                It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void UpdateOrganisationByIdAsync_NullUpdateViewModel_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using var controller = OrganisationControllerBuilder.Create().WithUpdateOrganisation(new Organisation()).Build();
                await controller.UpdateOrganisationByIdAsync(Guid.Empty, null);
            });
        }

        [Test]
        public async Task CreateOrganisationAsync_ServiceReturnsSuccess_Returns_CreatedAtAction()
        {
            var organisationId = Guid.NewGuid();
            using var controller = OrganisationControllerBuilder.Create().WithCreateOrganisationServiceReturningSuccess(organisationId).Build();

            var response = await controller.CreateOrganisationAsync(new CreateOrganisationRequestViewModel());

            response.Should().BeOfType<ActionResult<CreateOrganisationResponseViewModel>>();

            var expected = new CreatedAtActionResult(
                nameof(controller.GetByIdAsync).TrimAsync(),
                null, 
                new { id = organisationId },
                new CreateOrganisationResponseViewModel { OrganisationId = organisationId });

            var actual = response.Result;

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task CreateOrganisationAsync_ServiceReturnsFailure_Returns_BadRequest()
        {
            const string errorMessage = "Some Error Message Id";
            using var controller = OrganisationControllerBuilder.Create().WithCreateOrganisationServiceReturningFailure(errorMessage).Build();

            var response = await controller.CreateOrganisationAsync(new CreateOrganisationRequestViewModel());

            response.Should().BeOfType<ActionResult<CreateOrganisationResponseViewModel>>();

            var expected = new BadRequestObjectResult(new CreateOrganisationResponseViewModel
            {
                Errors = new[] { new ErrorMessageViewModel(errorMessage) }
            });

            var actual = response.Result;

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CreateOrganisationAsync_NullCreateOrganisationRequestViewModel_Throws()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using var controller = OrganisationControllerBuilder.Create().Build();
                await controller.CreateOrganisationAsync(null);
            });
        }
    }
}
