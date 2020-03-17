using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public DbSet<Organisation> Organisations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organisation>()
                .Property(b => b.Address)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x, new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Ignore}),
                    x => JsonConvert.DeserializeObject<Address>(x));
        }
    }
}
