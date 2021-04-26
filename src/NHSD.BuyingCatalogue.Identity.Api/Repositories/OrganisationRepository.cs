using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    internal sealed class OrganisationRepository : IOrganisationRepository
    {
        private readonly ApplicationDbContext context;

        public OrganisationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Organisation> GetByIdAsync(Guid id)
        {
            return await context.Organisations.Include(o => o.RelatedOrganisations).FirstOrDefaultAsync(org => org.OrganisationId == id);
        }
    }
}
