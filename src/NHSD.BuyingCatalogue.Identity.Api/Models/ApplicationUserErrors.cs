using NHSD.BuyingCatalogue.Identity.Common.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public static class ApplicationUserErrors
    {
        public static ErrorDetails FirstNameRequired()
        {
            return new("FirstNameRequired", nameof(ApplicationUser.FirstName));
        }

        public static ErrorDetails FirstNameTooLong()
        {
            return new("FirstNameTooLong", nameof(ApplicationUser.FirstName));
        }

        public static ErrorDetails LastNameRequired()
        {
            return new("LastNameRequired", nameof(ApplicationUser.LastName));
        }

        public static ErrorDetails LastNameTooLong()
        {
            return new("LastNameTooLong", nameof(ApplicationUser.LastName));
        }

        public static ErrorDetails PhoneNumberRequired()
        {
            return new("PhoneNumberRequired", nameof(ApplicationUser.PhoneNumber));
        }

        public static ErrorDetails PhoneNumberTooLong()
        {
            return new("PhoneNumberTooLong", nameof(ApplicationUser.PhoneNumber));
        }

        public static ErrorDetails EmailRequired()
        {
            return new("EmailRequired", "EmailAddress");
        }

        public static ErrorDetails EmailTooLong()
        {
            return new("EmailTooLong", "EmailAddress");
        }

        public static ErrorDetails EmailInvalidFormat()
        {
            return new("EmailInvalidFormat", "EmailAddress");
        }

        public static ErrorDetails EmailAlreadyExists()
        {
            return new("EmailAlreadyExists", "EmailAddress");
        }
    }
}
