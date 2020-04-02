using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models.Results;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public interface ICreateBuyerService
    {
        Task<Result<string>> CreateAsync(CreateBuyerRequest createBuyerRequest);
    }
}
