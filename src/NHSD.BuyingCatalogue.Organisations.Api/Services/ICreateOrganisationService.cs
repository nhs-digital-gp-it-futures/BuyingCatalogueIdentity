using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public interface ICreateOrganisationService
    {
        Task<Result<Guid>> CreateAsync(CreateOrganisationRequest request);
    }
}
