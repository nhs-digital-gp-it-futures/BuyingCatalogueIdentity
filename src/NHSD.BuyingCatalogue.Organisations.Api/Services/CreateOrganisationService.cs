using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Validators;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public sealed class CreateOrganisationService : ICreateOrganisationService
    {
        private readonly IOrganisationRepository organisationRepository;
        private readonly IOrganisationValidator organisationValidator;

        public CreateOrganisationService(IOrganisationRepository organisationRepository, IOrganisationValidator organisationValidator)
        {
            this.organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            this.organisationValidator = organisationValidator ?? throw new ArgumentNullException(nameof(organisationValidator));
        }

        public async Task<Result<Guid?>> CreateAsync(CreateOrganisationRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var organisation = new Organisation
            {
                Name = request.OrganisationName,
                OdsCode = request.OdsCode,
                PrimaryRoleId = request.PrimaryRoleId,
                LastUpdated = DateTime.UtcNow,
                CatalogueAgreementSigned = request.CatalogueAgreementSigned,
                Address = request.Address,
                OrganisationId = Guid.NewGuid(),
            };

            var validationResult = await organisationValidator.ValidateAsync(organisation);

            if (!validationResult.IsSuccess)
                return Result.Failure<Guid?>(validationResult.Errors);

            await organisationRepository.CreateOrganisationAsync(organisation);

            return Result.Success((Guid?)organisation.OrganisationId);
        }
    }
}
