using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Models.Results;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public interface ICreateBuyerService
    {
        Task<Result<string>> CreateAsync(CreateBuyerRequest createBuyerRequest);
    }
}
