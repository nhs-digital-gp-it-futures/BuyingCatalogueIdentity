using System;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Organisation> Organisations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
                throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.Entity<RelatedOrganisation>()
                         .HasOne(or => or.ChildOrganisation)
                         .WithMany()
                         .HasForeignKey(or => or.RelatedOrganisationId);

            modelBuilder.Entity<RelatedOrganisation>()
                        .HasOne(or => or.Organisation)
                        .WithMany(o => o.RelatedOrganisations)
                        .HasForeignKey(or => or.OrganisationId);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrganisationEntityTypeConfiguration());
        }
    }
}
