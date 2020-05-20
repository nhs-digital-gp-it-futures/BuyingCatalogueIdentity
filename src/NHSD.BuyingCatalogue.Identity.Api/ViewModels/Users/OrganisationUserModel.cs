namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users
{
    public sealed class OrganisationUserModel
    {
        public string UserId { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string EmailAddress { get; set; }

        public bool IsDisabled { get; set; }
    }
}
