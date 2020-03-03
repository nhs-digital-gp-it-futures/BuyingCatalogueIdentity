using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Organisations
{
    public sealed class GetAllOrganisationsViewModel
    {
        public IEnumerable<OrganisationViewModel> Organisations { get; set; }
    }
}
