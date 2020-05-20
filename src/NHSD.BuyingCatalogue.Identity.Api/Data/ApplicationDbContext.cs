using System;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<Organisation> Organisations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
                throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ApplicationUserEntityTypeConfiguration());
        }
    }
}
