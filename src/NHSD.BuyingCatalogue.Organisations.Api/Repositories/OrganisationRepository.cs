﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Organisations.Api.Data;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    internal sealed class OrganisationRepository : IOrganisationRepository
    {
        private readonly ApplicationDbContext context;

        public OrganisationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Organisation>> ListOrganisationsAsync()
        {
            return await context.Organisations.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Organisation> GetByIdAsync(Guid id)
        {
            return await context.Organisations.FirstOrDefaultAsync(org => org.OrganisationId == id);
        }

        public async Task<Organisation> GetByIdWithRelatedOrganisationsAsync(Guid id)
        {
            return await context.Organisations.Include(o => o.RelatedOrganisations).FirstOrDefaultAsync(org => org.OrganisationId == id);
        }

        public async Task<IEnumerable<Organisation>> GetUnrelatedOrganisations(Organisation organisation)
        {
            return await context.Organisations.Where(o => o != organisation && !o.ParentRelatedOrganisations.Contains(organisation)).ToListAsync();
        }

        public async Task<Organisation> GetByOdsCodeAsync(string odsCode)
        {
            return await context.Organisations.FirstOrDefaultAsync(org => org.OdsCode == odsCode);
        }

        public async Task CreateOrganisationAsync(Organisation organisation)
        {
            if (organisation is null)
                throw new ArgumentNullException(nameof(organisation));

            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Organisation organisation)
        {
            if (organisation is null)
                throw new ArgumentNullException(nameof(organisation));

            context.Organisations.Update(organisation);
            await context.SaveChangesAsync();
        }
    }
}
