namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Models
{
    public sealed class GetUser
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Name => $"{FirstName} {LastName}";

        public string PhoneNumber { get; init; }

        public string EmailAddress { get; init; }

        public bool Disabled { get; init; }

        public string OrganisationName { get; init; }

        public string SecurityStamp { get; init; }
    }
}
