using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OdsControllerBuilder
    {
        private IOdsRepository _odsRepository;

        private OdsControllerBuilder()
        {
            _odsRepository = Mock.Of<IOdsRepository>();
        }

        internal static OdsControllerBuilder Create()
        {
            return new OdsControllerBuilder();
        }

        internal OdsControllerBuilder WithGetByOdsCode(OdsOrganisation result)
        {
            var odsRepositoryMock = new Mock<IOdsRepository>();
            odsRepositoryMock.Setup(x => x.GetBuyerOrganisationByOdsCode(It.IsAny<string>())).ReturnsAsync(result);

            _odsRepository = odsRepositoryMock.Object;
            return this;
        }

        internal OdsControllerBuilder WithOdsRepository(IOdsRepository repository)
        {
            _odsRepository = repository;
            return this;
        }

        internal OdsController Build()
        {
            return new OdsController(_odsRepository);
        }
    }
}
