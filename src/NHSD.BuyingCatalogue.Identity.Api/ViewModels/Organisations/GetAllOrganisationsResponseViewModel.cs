using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Organisations
{
    public sealed class GetAllOrganisationsResponseViewModel
    {
        public IEnumerable<OrganisationViewModel> Organisations { get; set; }
    }
}
