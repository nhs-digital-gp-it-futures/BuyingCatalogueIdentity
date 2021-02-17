using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.Validators
{
    public sealed class OrganisationValidator : IOrganisationValidator
    {
        private readonly IOrganisationRepository organisationRepository;

        public OrganisationValidator(IOrganisationRepository organisationRepository)
        {
            this.organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
        }

        public async Task<Result> ValidateAsync(Organisation organisation)
        {
            if (organisation is null)
                throw new ArgumentNullException(nameof(organisation));

            var persistedOrganisation = await organisationRepository.GetByOdsCodeAsync(organisation.OdsCode);

            return persistedOrganisation is null
                ? Result.Success()
                : Result.Failure(new List<ErrorDetails> { OrganisationErrors.OrganisationAlreadyExists() });
        }
    }
}
