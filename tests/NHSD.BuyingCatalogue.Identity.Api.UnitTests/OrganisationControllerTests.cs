using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    public sealed class OrganisationControllerTests : Controller
    {
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

        [TestCase(null, null)]
        [TestCase("Organisation One", null)]
        [TestCase(null, "ODS 1")]
        [TestCase("Organisation One", "ODS 1")]
        public async Task GetAllAsync_SingleOrganisationExists_ReturnsTheOrganisation(string name, string ods)
        {
            var organisationId = Guid.NewGuid();

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>(){new Organisation(organisationId, name, ods)})
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
        }

        [Test]
        public async Task GetAllAsync_ListOfOrganisationsExist_ReturnsTheOrganisations()
        {
            var org1 = new Organisation(new Guid(), "Organisation 1", "ODS 1");
            var org2 = new Organisation(new Guid(), "Organisation 2", "ODS 2");
            var org3 = new Organisation(new Guid(), "Organisation 3", "ODS 3");

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithListOrganisation(new List<Organisation>() {org1, org2, org3})
                .Build();

            var result = await controller.GetAllAsync();

            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBe(null);

            var objectResult = result as OkObjectResult;
            objectResult.Value.Should().BeOfType<GetAllOrganisationsViewModel>();

            var organisationResult = objectResult.Value as GetAllOrganisationsViewModel;

            organisationResult.Organisations.Count().Should().Be(3);

            var organisationList = organisationResult.Organisations.ToList();

            organisationList[0].Name.Should().Be(org1.Name);
            organisationList[0].OdsCode.Should().Be(org1.OdsCode);

            organisationList[1].Name.Should().Be(org2.Name);
            organisationList[1].OdsCode.Should().Be(org2.OdsCode);

            organisationList[2].Name.Should().Be(org3.Name);
            organisationList[2].OdsCode.Should().Be(org3.OdsCode);
        }

        [Test]
        public async Task GetAllAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            Guid expectedId = Guid.NewGuid();

            var getAllOrganisations = new Mock<IOrganisationRepository>();
            getAllOrganisations.Setup(x => x.ListOrganisationsAsync())
                .ReturnsAsync(null as IEnumerable<Organisation>);

            using var controller = OrganisationControllerBuilder.Create()
                .WithOrganisationRepository(getAllOrganisations.Object)
                .Build();

            await controller.GetByIdAsync(expectedId);

            getAllOrganisations.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
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
            var organisation = new Organisation(Guid.NewGuid(), "org name", "ods-code");

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.Id);

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;

            objectResult.Value.Should().BeEquivalentTo(new OrganisationViewModel
            {
                OrganisationId = organisation.Id,
                Name = organisation.Name,
                OdsCode = organisation.OdsCode
            });
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

            mockGetOrganisation.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
