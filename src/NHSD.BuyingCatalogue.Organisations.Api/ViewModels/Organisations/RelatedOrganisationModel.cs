using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public class RelatedOrganisationModel
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }
    }
}
