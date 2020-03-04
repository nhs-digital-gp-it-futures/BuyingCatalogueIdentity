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
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Organisations;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    public sealed class OrganisationControllerTests : ControllerBase
    {
        private Mock<IOrganisationRepository> _mockOrganisationRepository;
        private OrganisationsController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockOrganisationRepository = new Mock<IOrganisationRepository>();
            _controller = new OrganisationsController(_mockOrganisationRepository.Object);
        }

        [Test]
        public async Task ShouldGetEmptyListOrganisations()
        {
            _mockOrganisationRepository.Setup(x => x.ListOrganisationsAsync()).ReturnsAsync(new List<Organisation>());

            var result = await _controller.GetAll();

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
        public async Task ShouldGetSingleOrganisation(string name, string ods)
        {
            _mockOrganisationRepository.Setup(x => x.ListOrganisationsAsync()).ReturnsAsync(new List<Organisation>()
            {
                new Organisation(new Guid(), name, ods)
            });

            var result = await _controller.GetAll();

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
        public async Task ShouldGetListOfOrganisations()
        {
            var org1 = new Organisation(new Guid(), "Organisation 1", "ODS 1");
            var org2 = new Organisation(new Guid(), "Organisation 2", "ODS 2");
            var org3 = new Organisation(new Guid(), "Organisation 3", "ODS 3");

            _mockOrganisationRepository.Setup(x => x.ListOrganisationsAsync()).ReturnsAsync(new List<Organisation>()
            {
                org1,
                org2,
                org3
            });

            var result = await _controller.GetAll();

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
    }
}
