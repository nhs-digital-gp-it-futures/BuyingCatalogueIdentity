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
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationValidator(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
        }

        public async Task<Result> ValidateAsync(Organisation organisation)
        {
            if (organisation is null)
                throw new ArgumentNullException(nameof(organisation));

            var persistedOrganisation = await _organisationRepository.GetByNameAsync(organisation.Name);

            return persistedOrganisation is null
                ? Result.Success()
                : Result.Failure(new List<ErrorDetails> {new ErrorDetails("OrganisationAlreadyExists")});
        }
    }
}
