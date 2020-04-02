using System;
using Microsoft.AspNetCore.Identity;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string FirstName { get; }

        public string LastName { get; }
        
        public string DisplayName => $"{FirstName} {LastName}";

        public Guid PrimaryOrganisationId { get; }
        
        public OrganisationFunction OrganisationFunction { get; }

        public bool Disabled { get; private set; }

        public bool CatalogueAgreementSigned { get; private set; }
        
        private ApplicationUser()
        {
        }

        private ApplicationUser(
            string userName,
            string firstName, 
            string lastName, 
            string phoneNumber, 
            string email, 
            OrganisationFunction organisationFunction,
            Guid primaryOrganisationId) : this()
        {
            if (userName is null)
                throw new ArgumentNullException(nameof(userName));

            if (firstName is null)
                throw new ArgumentNullException(nameof(firstName));

            if (lastName is null)
                throw new ArgumentNullException(nameof(lastName));

            if (email is null)
                throw new ArgumentNullException(nameof(email));

            UserName = userName.Trim();
            NormalizedUserName = UserName.ToUpperInvariant();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber?.Trim() ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email.Trim();
            NormalizedEmail = Email.ToUpperInvariant();
            OrganisationFunction = organisationFunction ?? throw new ArgumentNullException(nameof(organisationFunction));
            PrimaryOrganisationId = primaryOrganisationId;
            Disabled = false;
            CatalogueAgreementSigned = false;
        }

        public static ApplicationUser CreateBuyer(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            Guid primaryOrganisationId)
        {
            return new ApplicationUser(
                userName, 
                firstName, 
                lastName, 
                phoneNumber, 
                email, 
                OrganisationFunction.Buyer, 
                primaryOrganisationId);
        }

        public void MarkAsDisabled()
        {
            Disabled = true;
        }

        public void MarkCatalogueAgreementAsSigned()
        {
            CatalogueAgreementSigned = true;
        }
    }
}
