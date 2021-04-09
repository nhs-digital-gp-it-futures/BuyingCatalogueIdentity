using System;
using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class Organisation
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public Address Address { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public DateTime LastUpdated { get; set; }

        public ICollection<RelatedOrganisation> RelatedOrganisations { get; private set; }
    }
}
