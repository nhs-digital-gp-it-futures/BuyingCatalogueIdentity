using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ClaimsPrincipal claimsPrincipal;

        private IOrganisationRepository organisationRepository;
        private ICreateOrganisationService createOrganisationService;
        private IServiceRecipientRepository serviceRecipientRepository;

        private OrganisationControllerBuilder(Guid primaryOrganisationId)
        {
            createOrganisationService = Mock.Of<ICreateOrganisationService>();
            organisationRepository = Mock.Of<IOrganisationRepository>();
            serviceRecipientRepository = Mock.Of<IServiceRecipientRepository>();

            var claims = new[] { new Claim("primaryOrganisationId", primaryOrganisationId.ToString()) };
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        }

        internal static OrganisationControllerBuilder Create(Guid primaryOrganisationId = default)
        {
            return new(primaryOrganisationId);
        }

        internal OrganisationControllerBuilder WithServiceRecipients(IEnumerable<ServiceRecipient> result)
        {
            var mockServiceRecipientsRepository = new Mock<IServiceRecipientRepository>();
            mockServiceRecipientsRepository
                .Setup(r => r.GetServiceRecipientsByParentOdsCode(It.IsAny<string>()))
                .ReturnsAsync(result);

            serviceRecipientRepository = mockServiceRecipientsRepository.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithListOrganisation(IEnumerable<Organisation> result)
        {
            var mockListOrganisation = new Mock<IOrganisationRepository>();
            mockListOrganisation.Setup(r => r.ListOrganisationsAsync()).ReturnsAsync(result);

            organisationRepository = mockListOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithGetOrganisation(Organisation result)
        {
            var mockGetOrganisation = new Mock<IOrganisationRepository>();
            mockGetOrganisation.Setup(r => r.GetByIdAsync(result.OrganisationId)).ReturnsAsync(result);

            organisationRepository = mockGetOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithGetOrganisationWithRelatedOrganisations(Organisation result)
        {
            var mockGetWithRelated = new Mock<IOrganisationRepository>();
            mockGetWithRelated.Setup(r => r.GetByIdWithRelatedOrganisationsAsync(result.OrganisationId)).ReturnsAsync(result);

            organisationRepository = mockGetWithRelated.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithListOrganisationAndOrganisationWithRelatedOrganisations(IEnumerable<Organisation> result, Organisation org)
        {
            var mockListWithRelatedOrganisation = new Mock<IOrganisationRepository>();

            mockListWithRelatedOrganisation.Setup(r => r.ListOrganisationsAsync()).ReturnsAsync(result);
            mockListWithRelatedOrganisation.Setup(r => r.GetByIdWithRelatedOrganisationsAsync(org.OrganisationId)).ReturnsAsync(org);

            organisationRepository = mockListWithRelatedOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithGetByIdWithRelatedAndGetByIdForRelatedAndUpdateAsync(Organisation primaryOrg, Organisation relatedOrg)
        {
            var mock = new Mock<IOrganisationRepository>();
            mock.Setup(r => r.GetByIdWithRelatedOrganisationsAsync(primaryOrg.OrganisationId)).ReturnsAsync(primaryOrg);
            mock.Setup(r => r.GetByIdAsync(relatedOrg.OrganisationId)).ReturnsAsync(relatedOrg);
            mock.Setup(r => r.UpdateAsync(It.IsAny<Organisation>()));

            organisationRepository = mock.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithUpdateOrganisation(Organisation organisation)
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(organisation);
            repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Organisation>()));
            organisationRepository = repositoryMock.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithOrganisationRepository(IOrganisationRepository repository)
        {
            organisationRepository = repository;
            return this;
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningSuccess(Guid? result)
        {
            return WithCreateOrganisationServiceReturningResult(Result.Success(result));
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningFailure(string result)
        {
            return WithCreateOrganisationServiceReturningResult(Result.Failure<Guid?>(new ErrorDetails(result)));
        }

        internal OrganisationsController Build()
        {
            return new(organisationRepository, createOrganisationService, serviceRecipientRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal },
                },
            };
        }

        private OrganisationControllerBuilder WithCreateOrganisationServiceReturningResult(Result<Guid?> result)
        {
            WithCreateOrganisation();
            var service = new Mock<ICreateOrganisationService>();
            service
                .Setup(s => s.CreateAsync(It.IsAny<CreateOrganisationRequest>()))
                .ReturnsAsync(result);

            createOrganisationService = service.Object;
            return this;
        }

        private void WithCreateOrganisation()
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(r => r.CreateOrganisationAsync(It.IsAny<Organisation>()));
            organisationRepository = repositoryMock.Object;
        }
    }
}
