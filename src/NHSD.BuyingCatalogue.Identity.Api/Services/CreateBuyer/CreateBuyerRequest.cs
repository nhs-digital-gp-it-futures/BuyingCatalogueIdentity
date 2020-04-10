using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer
{
    public sealed class CreateBuyerRequest
    {
        public Guid PrimaryOrganisationId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string PhoneNumber { get; }

        public string EmailAddress { get; }

        public CreateBuyerRequest(Guid primaryOrganisationId, string firstName, string lastName, string phoneNumber, string emailAddress)
        {
            PrimaryOrganisationId = primaryOrganisationId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
        }

        private bool Equals(CreateBuyerRequest other)
        {
            return PrimaryOrganisationId.Equals(other.PrimaryOrganisationId) 
                   && string.Equals(FirstName, other.FirstName, StringComparison.Ordinal)
                   && string.Equals(LastName, other.LastName, StringComparison.Ordinal)
                   && string.Equals(PhoneNumber, other.PhoneNumber, StringComparison.Ordinal)
                   && string.Equals(EmailAddress, other.EmailAddress, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is CreateBuyerRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PrimaryOrganisationId, FirstName, LastName, PhoneNumber, EmailAddress);
        }
    }
}
