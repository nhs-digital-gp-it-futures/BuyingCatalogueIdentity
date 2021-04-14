using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    [Table("RelatedOrganisations")]
    public sealed class RelatedOrganisation
    {
        public Guid OrganisationId { get; set; }

        public Guid RelatedOrganisationId { get; set; }

        public Organisation Organisation { get; set; }

        public Organisation ChildOrganisation { get; set; }
    }
}
