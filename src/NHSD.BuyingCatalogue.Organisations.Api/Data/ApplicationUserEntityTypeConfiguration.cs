﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Data
{
    internal sealed class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.PrimaryOrganisationId)
                .HasColumnName("PrimaryOrganisationId");

            builder.Property(x => x.OrganisationFunction)
                .HasColumnName("OrganisationFunction")
                .HasConversion(x => x.DisplayName, value => OrganisationFunction.FromDisplayName(value));

            builder.Property(x => x.Disabled)
                .HasColumnName("Disabled");

            builder.Property(x => x.CatalogueAgreementSigned)
                .HasColumnName("CatalogueAgreementSigned");
        }
    }
}
