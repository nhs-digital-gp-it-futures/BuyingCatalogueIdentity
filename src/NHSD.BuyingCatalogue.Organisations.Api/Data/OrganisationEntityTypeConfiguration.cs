using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Data
{
    internal sealed class OrganisationEntityTypeConfiguration : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder
                .Property(entity => entity.Address)
                .HasConversion(
                    addressEntity => JsonSerializer.Serialize(
                        addressEntity,
                        new JsonSerializerOptions { IgnoreNullValues = true }),
                    addressEntity => JsonSerializer.Deserialize<Address>(
                        addressEntity,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
    }
}
