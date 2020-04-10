using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users
{
    public sealed class GetAllOrganisationUsersViewModel
    {
        public IEnumerable<OrganisationUserViewModel> Users { get; set; }
    }
}
