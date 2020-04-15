using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Validators
{
    public interface IApplicationUserValidator
    {
        Task<Result> ValidateAsync(ApplicationUser user);
    }
}
