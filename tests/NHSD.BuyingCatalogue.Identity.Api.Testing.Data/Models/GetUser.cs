namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Models
{
    public sealed class GetUser
    {
        public string FirstName { get; }

        public string LastName { get; }

        public string Name => $"{FirstName} {LastName}";

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public bool Disabled { get; set; }

        public string OrganisationName { get; set; }

        public string SecurityStamp { get; set; }
    }
}
