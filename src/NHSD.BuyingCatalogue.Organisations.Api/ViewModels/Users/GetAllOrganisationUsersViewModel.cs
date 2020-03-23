using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users
{
    public sealed class GetAllOrganisationUsersViewModel
    {
        public IEnumerable<OrganisationUserViewModel> Users { get; set; }
    }
}
