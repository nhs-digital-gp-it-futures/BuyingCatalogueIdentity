using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Validators
{
    public interface IOrganisationValidator
    {
        Task<Result> ValidateAsync(Organisation organisation);
    }
}
