using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class RelatedOrganisationModel
    {
        public Guid OrganisationId { get; init; }

        public string Name { get; init; }

        public string OdsCode { get; init; }
    }
}
