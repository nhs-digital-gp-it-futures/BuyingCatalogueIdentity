using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
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
                    addressEntity => JsonConvert.SerializeObject(addressEntity, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    }),
                    addressEntity => JsonConvert.DeserializeObject<Address>(addressEntity));
        }
    }
}
