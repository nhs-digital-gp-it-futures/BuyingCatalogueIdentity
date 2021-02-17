using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Data
{
    internal sealed class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .HasColumnName("FirstName");

            builder.Property(u => u.LastName)
                .HasColumnName("LastName");

            builder.Property(u => u.PrimaryOrganisationId)
                .HasColumnName("PrimaryOrganisationId");

            builder.Property(u => u.OrganisationFunction)
                .HasColumnName("OrganisationFunction")
                .HasConversion(o => o.DisplayName, value => OrganisationFunction.FromDisplayName(value));

            builder.Property(u => u.Disabled)
                .HasColumnName("Disabled");

            builder.Property(u => u.CatalogueAgreementSigned)
                .HasColumnName("CatalogueAgreementSigned");
        }
    }
}
