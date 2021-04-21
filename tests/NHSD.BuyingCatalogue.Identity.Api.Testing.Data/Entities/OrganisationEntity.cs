using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public sealed class OrganisationEntity : EntityBase
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public string Address
        {
            get { return JsonConvert.SerializeObject(AddressObject); }
        }

        public bool CatalogueAgreementSigned { get; set; }

        public DateTime LastUpdated { get; set; }

        public Address AddressObject { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.Organisations
            (
                OrganisationId,
                Name,
                OdsCode,
                PrimaryRoleId,
                Address,
                CatalogueAgreementSigned,
                LastUpdated
            )
            VALUES
            (
                @OrganisationId,
                @Name,
                @OdsCode,
                @PrimaryRoleId,
                @Address,
                @CatalogueAgreementSigned,
                @LastUpdated
            );";

        protected override string GetSql { get; }

        public static async Task<OrganisationEntity> GetByNameAsync(string connectionString, string organisationName)
        {
            const string sql = @"
                  SELECT TOP (1) OrganisationId, Name
                    FROM dbo.Organisations
                   WHERE Name = @organisationName
                ORDER BY Name ASC";

            return await SqlRunner.FetchSingleResultAsync<OrganisationEntity>(
                connectionString,
                sql,
                new { organisationName });
        }

        public async Task InsertRelatedOrganisation(string connectionString, Guid relatedOrganisationId)
        {
            const string sql = @"
            INSERT INTO dbo.RelatedOrganisations
            (
	            OrganisationId,
	            RelatedOrganisationId
            )
            VALUES
            (
	            @OrganisationId,
	            @relatedOrganisationId
            );";

            await SqlRunner.ExecuteAsync(connectionString, sql, new { OrganisationId, relatedOrganisationId });
        }
    }
}
