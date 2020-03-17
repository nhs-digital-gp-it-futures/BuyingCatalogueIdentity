using Newtonsoft.Json;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers
{
    public sealed class OrganisationUserViewModel
    {
        [JsonProperty("userID")]
        public string UserId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }
    }
}
