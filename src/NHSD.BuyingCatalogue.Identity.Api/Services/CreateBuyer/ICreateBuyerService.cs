using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer
{
    public interface ICreateBuyerService
    {
        Task<Result<string>> CreateAsync(CreateBuyerRequest createBuyerRequest);
    }
}
