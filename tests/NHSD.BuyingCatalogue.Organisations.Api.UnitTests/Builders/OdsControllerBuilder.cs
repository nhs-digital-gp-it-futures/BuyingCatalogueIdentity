using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OdsControllerBuilder
    {
        private IOdsRepository odsRepository;

        private OdsControllerBuilder()
        {
            odsRepository = Mock.Of<IOdsRepository>();
        }

        internal static OdsControllerBuilder Create()
        {
            return new();
        }

        internal OdsControllerBuilder WithGetByOdsCode(OdsOrganisation result)
        {
            var odsRepositoryMock = new Mock<IOdsRepository>();
            odsRepositoryMock.Setup(r => r.GetBuyerOrganisationByOdsCodeAsync(It.IsAny<string>())).ReturnsAsync(result);

            odsRepository = odsRepositoryMock.Object;
            return this;
        }

        internal OdsControllerBuilder WithOdsRepository(IOdsRepository repository)
        {
            odsRepository = repository;
            return this;
        }

        internal OdsController Build()
        {
            return new(odsRepository);
        }
    }
}
