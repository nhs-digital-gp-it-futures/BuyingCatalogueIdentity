using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Comparers
{
    internal sealed class ApplicationUserEditableInformationComparer : IEqualityComparer<ApplicationUser>
    {
        private static readonly Lazy<ApplicationUserEditableInformationComparer> _instance =
            new Lazy<ApplicationUserEditableInformationComparer>(() =>
                new ApplicationUserEditableInformationComparer());

        public static ApplicationUserEditableInformationComparer Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private ApplicationUserEditableInformationComparer()
        {
        }

        public bool Equals(ApplicationUser original, ApplicationUser comparison)
        {
            if (original is null)
                return comparison is null;

            if (comparison is null)
                return false;

            if (original.GetType() != comparison.GetType())
                return false;

            return string.Equals(original.FirstName, comparison.FirstName, StringComparison.Ordinal) 
                && string.Equals(original.LastName, comparison.LastName, StringComparison.Ordinal)
                && string.Equals(original.PhoneNumber, comparison.PhoneNumber, StringComparison.Ordinal)
                && string.Equals(original.Email, comparison.Email, StringComparison.Ordinal)
                && string.Equals(original.NormalizedEmail, comparison.NormalizedEmail, StringComparison.Ordinal)
                && Equals(original.PrimaryOrganisationId, comparison.PrimaryOrganisationId)
                && string.Equals(original.OrganisationFunction, comparison.OrganisationFunction, StringComparison.Ordinal)
                && Equals(original.Disabled, comparison.Disabled)
                && Equals(original.CatalogueAgreementSigned, comparison.CatalogueAgreementSigned);
        }

        public int GetHashCode(ApplicationUser obj)
        {
            return HashCode.Combine(
                obj.FirstName, 
                obj.LastName, 
                obj.PhoneNumber, 
                obj.Email, 
                obj.PrimaryOrganisationId, 
                obj.OrganisationFunction, 
                obj.Disabled,
                obj.CatalogueAgreementSigned);
        }
    }
}
