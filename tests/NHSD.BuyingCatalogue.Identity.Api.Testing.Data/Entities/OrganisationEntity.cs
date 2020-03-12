using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public sealed class OrganisationEntity : EntityBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public DateTime LastUpdated { get; set; }

        protected override string InsertSql => $@"
                                INSERT INTO dbo.Organisations
                                (Id,
                                Name,
                                OdsCode,
                                LastUpdated)
                                VALUES
                                    (@Id,
                                     @Name,
                                     @OdsCode,
                                     @LastUpdated)";

        public static async Task<OrganisationEntity> GetByNameAsync(string connectionString, string organisationName) =>
            await SqlRunner.FetchSingleResultAsync<OrganisationEntity>(connectionString, $@"SELECT TOP (1)
                                    Id,
                                    Name
                                    FROM Organisations
                                    WHERE Name = @organisationName
                                    ORDER BY Name ASC", new{organisationName});
    }
}
