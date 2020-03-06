using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public sealed class OrganisationEntity : EntityBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public DateTime LastUpdated { get; set; }

        protected override string InsertSql => $@"
                                INSERT INTO [dbo].[Organisations]
                                ([Id]
                                ,[Name]
                                ,[OdsCode]
                                ,[LastUpdated])
                                VALUES
                                    (@Id
                                    ,@Name
                                    ,@OdsCode
                                    ,@LastUpdated)";
    }
}
