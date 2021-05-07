using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OdsControllerTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public static async Task GetByOdsCodeAsync_InvalidOdsCode_ReturnsNotFound(string invalidCode)
        {
            var mockOdsRepository = new Mock<IOdsRepository>();
            var mockOrganisationRepository = new Mock<IOrganisationRepository>();
            using var controller = new OdsController(
                mockOdsRepository.Object,
                mockOrganisationRepository.Object);

            var result = await controller.GetByOdsCodeAsync(invalidCode);

            mockOdsRepository.Verify(r => r.GetBuyerOrganisationByOdsCodeAsync(It.IsAny<string>()), Times.Never);
            mockOrganisationRepository.Verify(r => r.GetByOdsCodeAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static async Task GetByOdsCodeAsync_OdsOrganisationDoesNotExist_ReturnsNotFound()
        {
            var organisation = OrganisationBuilder.Create(1).WithOdsCode("some-code").Build();
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(null, organisation)
                .Build();

            var result = await controller.GetByOdsCodeAsync(organisation.OdsCode);

            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static async Task GetByOdsCodeAsync_OrganisationDoesNotExist_ReturnsNotFound()
        {
            var buyerOrganisation = OdsOrganisationBuilder.Create(1, true).Build();
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(buyerOrganisation, null)
                .Build();

            var result = await controller.GetByOdsCodeAsync(buyerOrganisation.OdsCode);

            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static async Task GetByOdsCodeAsync_OrganisationIsNotBuyerOrganisation_ReturnsNotAccepted()
        {
            var nonBuyerOrganisation = OdsOrganisationBuilder.Create(1).Build();
            var organisation = OrganisationBuilder.Create(1).WithOdsCode(nonBuyerOrganisation.OdsCode).Build();
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(nonBuyerOrganisation, organisation)
                .Build();

            var response = await controller.GetByOdsCodeAsync(nonBuyerOrganisation.OdsCode);

            response.As<StatusCodeResult>().StatusCode.Should().Be((int)StatusCodes.Status406NotAcceptable);
        }

        [Test]
        public static async Task GetByOdsCodeAsync_OrganisationExists_ReturnsActiveBuyerOrganisation()
        {
            var buyerOrganisation = OdsOrganisationBuilder.Create(1, true).Build();
            var organisation = OrganisationBuilder.Create(1).WithOdsCode(buyerOrganisation.OdsCode).Build();

            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(buyerOrganisation, organisation)
                .Build();

            var response = await controller.GetByOdsCodeAsync(buyerOrganisation.OdsCode);

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeEquivalentTo(
                new OdsOrganisationModel
                {
                    OdsCode = buyerOrganisation.OdsCode,
                    OrganisationId = organisation.OrganisationId,
                    OrganisationName = buyerOrganisation.OrganisationName,
                    PrimaryRoleId = buyerOrganisation.PrimaryRoleId,
                });
        }

        [Test]
        public static async Task GetByOdsCodeAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            const string odsCode = "123";

            var odsRepositoryMock = new Mock<IOdsRepository>();
            odsRepositoryMock.Setup(r => r.GetBuyerOrganisationByOdsCodeAsync(It.IsAny<string>()))
                .ReturnsAsync((OdsOrganisation)null);

            using var controller = OdsControllerBuilder.Create()
                .WithOdsRepository(odsRepositoryMock.Object)
                .Build();

            await controller.GetByOdsCodeAsync(odsCode);

            odsRepositoryMock.Verify(r => r.GetBuyerOrganisationByOdsCodeAsync(odsCode));
        }
    }
}
