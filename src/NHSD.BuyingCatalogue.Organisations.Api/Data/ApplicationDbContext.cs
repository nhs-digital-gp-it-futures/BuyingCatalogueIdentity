﻿using System;
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

            modelBuilder.Entity<Organisation>()
                .HasMany(o => o.RelatedOrganisations)
                .WithMany(o => o.ParentRelatedOrganisations)
                .UsingEntity<RelatedOrganisation>(
                relatedOrganisation => relatedOrganisation
                                         .HasOne(ro => ro.ChildOrganisation)
                                         .WithMany()
                                         .HasConstraintName("FK_RelatedOrganisations_RelatedOrganisationId")
                                         .OnDelete(DeleteBehavior.Cascade),
                relatedOrganisation => relatedOrganisation
                                         .HasOne(ro => ro.Organisation)
                                         .WithMany()
                                         .HasConstraintName("FK_RelatedOrganisations_OrganisationId")
                                         .OnDelete(DeleteBehavior.Cascade));

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrganisationEntityTypeConfiguration());
        }
    }
}
