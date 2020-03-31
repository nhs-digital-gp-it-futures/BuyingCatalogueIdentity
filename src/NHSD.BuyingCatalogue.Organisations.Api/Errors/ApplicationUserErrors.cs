using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Errors
{
    public static class ApplicationUserErrors
    {
        public static Error FirstNameRequired()
        {
            return new Error("FirstNameRequired", nameof(ApplicationUser.FirstName));
        }

        public static Error FirstNameTooLong()
        {
            return new Error("FirstNameTooLong", nameof(ApplicationUser.FirstName));
        }

        public static Error LastNameRequired()
        {
            return new Error("LastNameRequired", nameof(ApplicationUser.LastName));
        }

        public static Error LastNameTooLong()
        {
            return new Error("LastNameTooLong", nameof(ApplicationUser.LastName));
        }

        public static Error PhoneNumberRequired()
        {
            return new Error("PhoneNumberRequired", nameof(ApplicationUser.PhoneNumber));
        }

        public static Error EmailRequired()
        {
            return new Error("EmailRequired", nameof(ApplicationUser.Email));
        }

        public static Error EmailTooLong()
        {
            return new Error("EmailTooLong", nameof(ApplicationUser.Email));
        }

        public static Error EmailInvalidFormat()
        {
            return new Error("EmailInvalidFormat", nameof(ApplicationUser.Email));
        }
        
        public static Error EmailAlreadyExists()
        {
            return new Error("EmailAlreadyExists", nameof(ApplicationUser.Email));
        }
    }
}
