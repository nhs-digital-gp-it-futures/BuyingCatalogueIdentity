using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    public sealed class OrganisationControllerTests
    {
        private Address address1 = new Address
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

        private Address address2 = new Address
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
                    new Organisation()
                    {
                        Id = organisationId,
                        Name = name,
                        OdsCode = ods,
                        PrimaryRoleId = primaryRoleId,
                        CatalogueAgreementSigned = catalogueAgreementSigned,
                        Address = hasAddress == false ? null : JsonConvert.SerializeObject(address1)
                    }
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
                organisationList[0].Location.Should().BeEquivalentTo(address1);
            }
            else
            {
                organisationList[0].Location.Should().BeNull();
            }
        }

        [Test]
        public async Task GetAllAsync_ListOfOrganisationsExist_ReturnsTheOrganisations()
        {
            var org1 = new Organisation
            {
                Id = Guid.NewGuid(),
                Name = "Organisation 1",
                OdsCode = "ODS 1",
                PrimaryRoleId = "ID 1",
                CatalogueAgreementSigned = false,
                Address = JsonConvert.SerializeObject(address1)
            };
            var org2 = new Organisation
            {
                Id = Guid.NewGuid(),
                Name = "Organisation 2",
                OdsCode = "ODS 2",
                PrimaryRoleId = "ID 2",
                CatalogueAgreementSigned = true,
                Address = JsonConvert.SerializeObject(address2)
            };

            var org3 = new Organisation
            {
                Id = Guid.NewGuid(),
                Name = "Organisation 3",
                OdsCode = "ODS 3",
                PrimaryRoleId = "ID 3",
                CatalogueAgreementSigned = true,
                Address = null
            };

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

            organisationList[0].Name.Should().Be(org1.Name);
            organisationList[0].OdsCode.Should().Be(org1.OdsCode);
            organisationList[0].PrimaryRoleId.Should().Be(org1.PrimaryRoleId);
            organisationList[0].CatalogueAgreementSigned.Should().Be(org1.CatalogueAgreementSigned);
            organisationList[0].Location.Should().BeEquivalentTo(address1);

            organisationList[1].Name.Should().Be(org2.Name);
            organisationList[1].OdsCode.Should().Be(org2.OdsCode);
            organisationList[1].PrimaryRoleId.Should().Be(org2.PrimaryRoleId);
            organisationList[1].CatalogueAgreementSigned.Should().Be(org2.CatalogueAgreementSigned);
            organisationList[1].Location.Should().BeEquivalentTo(address2);

            organisationList[2].Name.Should().Be(org3.Name);
            organisationList[2].OdsCode.Should().Be(org3.OdsCode);
            organisationList[2].PrimaryRoleId.Should().Be(org3.PrimaryRoleId);
            organisationList[2].CatalogueAgreementSigned.Should().Be(org3.CatalogueAgreementSigned);
            organisationList[2].Location.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            var getAllOrganisations = new Mock<IOrganisationRepository>();
            getAllOrganisations.Setup(x => x.ListOrganisationsAsync())
                .ReturnsAsync(null as IEnumerable<Organisation>);

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
            var organisation = new Organisation
            {
                Id = Guid.NewGuid(),
                Name = "org name",
                OdsCode = "ods-code",
                PrimaryRoleId = "ID 1",
                CatalogueAgreementSigned = true,
                Address = JsonConvert.SerializeObject(address1)
            };

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.Id);

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;

            objectResult.Value.Should().BeEquivalentTo(organisation,
                conf => conf.Excluding(c => c.Id).Excluding(c => c.Address).Excluding(c => c.LastUpdated));
        }

        [Test]
        public async Task GetByIdAsync_OrganisationAddressIsNull_ReturnsOrganisationWithNullAddress()
        {
            var organisation = new Organisation
            {
                Id = Guid.NewGuid(),
                Name = "org name",
                OdsCode = "ods-code",
                PrimaryRoleId = "ID 1",
                CatalogueAgreementSigned = true,
                Address = null
            };

            using var controller = OrganisationControllerBuilder
                .Create()
                .WithGetOrganisation(organisation)
                .Build();

            var result = await controller.GetByIdAsync(organisation.Id);

            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;

            objectResult.Value.Should().BeEquivalentTo(organisation,
                conf => conf.Excluding(c => c.Id).Excluding(c => c.Address).Excluding(c => c.LastUpdated));
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
