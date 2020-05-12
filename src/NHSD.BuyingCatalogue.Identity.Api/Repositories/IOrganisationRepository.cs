using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    public interface IOrganisationRepository
    {
        Task<Organisation> GetByIdAsync(Guid id);
    }
}
