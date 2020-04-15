using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<ApplicationUser>> FindByOrganisationIdAsync(Guid organisationId);

        Task<ApplicationUser> GetByEmailAsync(string email);

        Task<ApplicationUser> GetByIdAsync(string userId);

        Task CreateUserAsync(ApplicationUser user);

        Task UpdateAsync(ApplicationUser user);
    }
}
