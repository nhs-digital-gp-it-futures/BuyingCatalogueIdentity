using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Validators
{
    public interface IApplicationUserValidator
    {
        Task<Result> ValidateAsync(ApplicationUser user);
    }
}
