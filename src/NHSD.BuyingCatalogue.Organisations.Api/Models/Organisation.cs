using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class Organisation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public Address Address { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
