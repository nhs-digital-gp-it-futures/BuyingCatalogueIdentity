using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OrganisationControllerBuilder
    {
        private IOrganisationRepository _organisationRepository;

        private OrganisationControllerBuilder()
        {
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

        public OrganisationControllerBuilder WithOrganisationRepository(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
            return this;
        }

        internal OrganisationsController Build()
        {
            return new OrganisationsController(_organisationRepository);
        }
    }
}
