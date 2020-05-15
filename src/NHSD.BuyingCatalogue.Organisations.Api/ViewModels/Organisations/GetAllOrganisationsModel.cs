using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class GetAllOrganisationsModel
    {
        public IEnumerable<OrganisationModel> Organisations { get; set; }
    }
}
