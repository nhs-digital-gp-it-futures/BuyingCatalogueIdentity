using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public interface ICreateOrganisationService
    {
        Task<Result<string>> CreateAsync(CreateOrganisationRequest request);
    }
}
