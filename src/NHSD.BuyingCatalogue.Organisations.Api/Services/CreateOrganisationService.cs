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
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationValidator _organisationValidator;

        public CreateOrganisationService(IOrganisationRepository organisationRepository, IOrganisationValidator organisationValidator)
        {
            _organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            _organisationValidator = organisationValidator ?? throw new ArgumentNullException(nameof(organisationValidator));
        }

        public async Task<Result<string>> CreateAsync(CreateOrganisationRequest request)
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
                OrganisationId = Guid.NewGuid()
            };

            var validationResult = await _organisationValidator.ValidateAsync(organisation);

            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Errors);

            await _organisationRepository.CreateOrganisationAsync(organisation);

            return Result.Success(organisation.OrganisationId.ToString());
        }
    }
}
