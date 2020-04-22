using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OrganisationControllerBuilder
    {
        private IOrganisationRepository _organisationRepository;
        private ICreateOrganisationService _createOrganisationService;

        private OrganisationControllerBuilder()
        {
            _createOrganisationService = Mock.Of<ICreateOrganisationService>();
            _organisationRepository = Mock.Of<IOrganisationRepository>();
        }

        internal static OrganisationControllerBuilder Create()
        {
            return new OrganisationControllerBuilder();
        }

        internal OrganisationControllerBuilder WithListOrganisation(IEnumerable<Organisation> result)
        {
            var mockListOrganisation = new Mock<IOrganisationRepository>();
            mockListOrganisation.Setup(x => x.ListOrganisationsAsync()).ReturnsAsync(result);

            _organisationRepository = mockListOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithGetOrganisation(Organisation result)
        {
            var mockGetOrganisation = new Mock<IOrganisationRepository>();
            mockGetOrganisation.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(result);

            _organisationRepository = mockGetOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithUpdateOrganisation(Organisation organisation)
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(organisation);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Organisation>()));
            _organisationRepository = repositoryMock.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithOrganisationRepository(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
            return this;
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningSuccess(Guid result)
        {
            return WithCreateOrganisationServiceReturningResult(Result.Success(result));
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningFailure(string result)
        {
            return WithCreateOrganisationServiceReturningResult(Result.Failure<Guid>(new ErrorDetails(result)));
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningResult(Result<Guid> result)
        {
            WithCreateOrganisation();
            var createOrganisationService = new Mock<ICreateOrganisationService>();
            createOrganisationService.Setup(s => s.CreateAsync(It.IsAny<CreateOrganisationRequest>()))
                .ReturnsAsync(result);
            _createOrganisationService = createOrganisationService.Object;
            return this;
        }

        private void WithCreateOrganisation()
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(x => x.CreateOrganisationAsync(It.IsAny<Organisation>()));
            _organisationRepository = repositoryMock.Object;
        }

        internal OrganisationsController Build()
        {
            return new OrganisationsController(_organisationRepository, _createOrganisationService);
        }
    }
}
