using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    internal sealed class OrganisationRepository : IOrganisationRepository
    {
        private readonly ApplicationDbContext _context;

        public OrganisationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Organisation> GetByIdAsync(Guid id)
        {
            return await _context.Organisations.FindAsync(id);
        }
    }
}
