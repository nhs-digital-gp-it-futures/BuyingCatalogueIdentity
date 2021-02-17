using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer
{
    public sealed class CreateBuyerRequest : IEquatable<CreateBuyerRequest>
    {
        public CreateBuyerRequest(
            Guid primaryOrganisationId,
            string firstName,
            string lastName,
            string phoneNumber,
            string emailAddress)
        {
            PrimaryOrganisationId = primaryOrganisationId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
        }

        public Guid PrimaryOrganisationId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string PhoneNumber { get; }

        public string EmailAddress { get; }

        public bool Equals(CreateBuyerRequest other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return PrimaryOrganisationId.Equals(other.PrimaryOrganisationId)
                && string.Equals(FirstName, other.FirstName, StringComparison.Ordinal)
                && string.Equals(LastName, other.LastName, StringComparison.Ordinal)
                && string.Equals(PhoneNumber, other.PhoneNumber, StringComparison.Ordinal)
                && string.Equals(EmailAddress, other.EmailAddress, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CreateBuyerRequest);
        }

        public override int GetHashCode() => HashCode.Combine(PrimaryOrganisationId, FirstName, LastName, PhoneNumber, EmailAddress);
    }
}
