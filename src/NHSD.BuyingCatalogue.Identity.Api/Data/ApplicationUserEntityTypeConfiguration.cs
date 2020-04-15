using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Data
{
    internal sealed class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName)
                .HasColumnName("FirstName");

            builder.Property(x => x.LastName)
                .HasColumnName("LastName");

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
