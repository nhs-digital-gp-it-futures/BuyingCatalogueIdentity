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

        Task<Organisation> GetByIdWithRelatedOrganisationsAsync(Guid id, bool requireChildrenToLoad = true);

        Task<Organisation> GetByOdsCodeAsync(string odsCode);

        Task CreateOrganisationAsync(Organisation organisation);

        Task UpdateAsync(Organisation organisation);
    }
}
