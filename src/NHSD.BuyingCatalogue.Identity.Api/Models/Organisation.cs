using System;
using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public sealed class Organisation
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public ICollection<Organisation> RelatedOrganisations { get; } = new List<Organisation>();

        public ICollection<Organisation> ParentRelatedOrganisations { get; } = new List<Organisation>();
    }
}
