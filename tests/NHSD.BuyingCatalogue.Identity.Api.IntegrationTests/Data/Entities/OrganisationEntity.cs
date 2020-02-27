using System;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data.Entities
{
    public sealed class OrganisationEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
