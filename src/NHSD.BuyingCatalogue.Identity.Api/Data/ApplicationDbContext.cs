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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Entity<Organisation>()
                        .HasMany(o => o.RelatedOrganisations)
                        .WithMany(o => o.ParentRelatedOrganisations)
                        .UsingEntity<RelatedOrganisation>(
                        relatedOrganisation => relatedOrganisation
                                                 .HasOne(ro => ro.ChildOrganisation)
                                                 .WithMany()
                                                 .HasForeignKey(ro => ro.RelatedOrganisationId)
                                                 .HasConstraintName("FK_RelatedOrganisations_RelatedOrganisationId")
                                                 .OnDelete(DeleteBehavior.Cascade),
                        relatedOrganisation => relatedOrganisation
                                                 .HasOne(ro => ro.Organisation)
                                                 .WithMany()
                                                 .HasForeignKey(ro => ro.OrganisationId)
                                                 .HasConstraintName("FK_RelatedOrganisations_OrganisationId")
                                                 .OnDelete(DeleteBehavior.Cascade),
                        relatedOrganisation => relatedOrganisation
                             .HasKey(ro => new { ro.OrganisationId, ro.RelatedOrganisationId }));

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserEntityTypeConfiguration());
        }
    }
}
