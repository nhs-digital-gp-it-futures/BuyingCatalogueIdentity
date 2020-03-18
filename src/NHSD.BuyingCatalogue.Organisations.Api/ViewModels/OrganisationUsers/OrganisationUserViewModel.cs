using System;
using Newtonsoft.Json;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers
{
    public sealed class OrganisationUserViewModel
    {
        public string UserId { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string EmailAddress { get; set; }

        public bool IsDisabled { get; set; }

        [JsonIgnore]
        public Guid OrganisationId { get; set; }
    }
}
