using System.ComponentModel.DataAnnotations.Schema;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    [Table("RelatedOrganisations")]
    public sealed class RelatedOrganisation
    {
        [ForeignKey("OrganisationId")]
        public Organisation Organisation { get; set; }

        [ForeignKey("RelatedOrganisationId")]
        public Organisation ChildOrganisation { get; set; }
    }
}
