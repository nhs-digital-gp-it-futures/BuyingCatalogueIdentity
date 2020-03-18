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
    public sealed class OrganisationControllerTests
    {
        private readonly Address _address1 = new Address
        {
            Line1 = "8",
            Line2 = "Sands Lane",
            Line3 = "Shopping Mall",
            Line4 = "Horsforth",
            Town = "Leeds",
            County = "West Yorshire",
            Postcode = "LS3 4FD",
            Country = "West Yorkshire"
        };

        private readonly Address _address2 = new Address
        {
            Line1 = "2",
            Line2 = "Brick Lane",
            Line3 = "City Centre Flats",
            Line4 = "City Centre",
            Town = "Leeds",
            County = "West Yorshire",
            Postcode = "LS1 1AE",
            Country = "West Yorkshire"
        };

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
            var organisationId = Guid.NewGuid();

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
            var org1 = OrganisationBuilder.Create(1).WithCatalogueAgreementSigned(false).WithAddress(_address1).Build();

            var org2 = OrganisationBuilder.Create(2).WithAddress(_address2).Build();

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
    }
}
