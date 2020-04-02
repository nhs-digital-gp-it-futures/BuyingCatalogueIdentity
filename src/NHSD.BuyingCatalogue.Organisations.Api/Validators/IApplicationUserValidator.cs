using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Models.Results;

namespace NHSD.BuyingCatalogue.Organisations.Api.Validators
{
    public interface IApplicationUserValidator
    {
        Task<Result> ValidateAsync(ApplicationUser user);
    }
}
