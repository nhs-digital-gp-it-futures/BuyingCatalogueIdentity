using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    public interface IOrganisationRepository
    {
        Task<IEnumerable<Organisation>> ListOrganisationsAsync();

        Task<Organisation> GetByIdAsync(Guid id);

        Task UpdateAsync(Organisation organisation);
    }
}
