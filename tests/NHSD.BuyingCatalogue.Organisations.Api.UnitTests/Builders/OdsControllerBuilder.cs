using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OdsControllerBuilder
    {
        private IOdsRepository odsRepository;
        private IOrganisationRepository orgRepository;

        private OdsControllerBuilder()
        {
            odsRepository = Mock.Of<IOdsRepository>();
            orgRepository = Mock.Of<IOrganisationRepository>();
        }

        internal static OdsControllerBuilder Create()
        {
            return new();
        }

        internal OdsControllerBuilder WithGetByOdsCode(OdsOrganisation result, Organisation organisation)
        {
            var odsRepositoryMock = new Mock<IOdsRepository>();
            if (result != null)
            {
                odsRepositoryMock.Setup(r => r.GetBuyerOrganisationByOdsCodeAsync(result.OdsCode)).ReturnsAsync(result);
            }
            odsRepository = odsRepositoryMock.Object;

            var orgRepositoryMock = new Mock<IOrganisationRepository>();
            if (organisation != null)
            {
                orgRepositoryMock
                    .Setup(o => o.GetByOdsCodeAsync(result == null ? organisation.OdsCode : result.OdsCode))
                    .ReturnsAsync(organisation);
            }
            orgRepository = orgRepositoryMock.Object;

            return this;
        }

        internal OdsControllerBuilder WithOdsRepository(IOdsRepository repository)
        {
            odsRepository = repository;
            return this;
        }

        internal OdsController Build()
        {
            return new(odsRepository, orgRepository);
        }
    }
}
