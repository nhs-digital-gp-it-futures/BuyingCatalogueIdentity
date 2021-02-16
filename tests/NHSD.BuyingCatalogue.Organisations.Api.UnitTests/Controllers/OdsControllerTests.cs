using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
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
        public static async Task GetByOdsCodeAsync_OrganisationDoesNotExist_ReturnsNotFound(string odsCode)
        {
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(null)
                .Build();

            var result = await controller.GetByOdsCodeAsync(odsCode);

            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public static async Task GetByOdsCodeAsync_OrganisationIsNotBuyerOrganisation_ReturnsNotAccepted()
        {
            var nonBuyerOrganisation = OdsOrganisationBuilder.Create(1).Build();
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(nonBuyerOrganisation)
                .Build();

            // ReSharper disable StringLiteralTypo
            var response = await controller.GetByOdsCodeAsync("dolor eternum");

            // ReSharper restore StringLiteralTypo
            response.Should().BeOfType<StatusCodeResult>();

            response.Should().BeEquivalentTo(new StatusCodeResult(StatusCodes.Status406NotAcceptable));
        }

        [Test]
        public static async Task GetByOdsCodeAsync_OrganisationExists_ReturnsActiveBuyerOrganisation()
        {
            var buyerOrganisation = OdsOrganisationBuilder.Create(1, true).Build();

            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(buyerOrganisation)
                .Build();

            var response = await controller.GetByOdsCodeAsync(buyerOrganisation.OdsCode);

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeEquivalentTo(
                buyerOrganisation,
                options => options.Excluding(o => o.IsActive).Excluding(o => o.IsBuyerOrganisation));
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
