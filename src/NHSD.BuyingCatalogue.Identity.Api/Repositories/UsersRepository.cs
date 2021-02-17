using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext context;

        public UsersRepository(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ApplicationUser>> FindByOrganisationIdAsync(Guid organisationId)
        {
            return await context.Users.Where(u => u.PrimaryOrganisationId == organisationId).ToListAsync();
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            if (email is null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await context.Users.SingleOrDefaultAsync(applicationUser => applicationUser.Email == email);
        }

        public async Task<ApplicationUser> GetByIdAsync(string userId)
            => await context.Users.FindAsync(userId);

        public async Task CreateUserAsync(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
