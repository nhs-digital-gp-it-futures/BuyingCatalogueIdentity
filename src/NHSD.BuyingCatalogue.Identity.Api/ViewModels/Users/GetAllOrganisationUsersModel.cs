using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users
{
    public sealed class GetAllOrganisationUsersModel
    {
        public IEnumerable<OrganisationUserModel> Users { get; set; }
    }
}
