using Newtonsoft.Json;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers
{
    public sealed class GetAllOrganisationUsersViewModel
    {
        [JsonProperty("users")]
        public OrganisationUserViewModel[] Users { get; set; }
    }
}
