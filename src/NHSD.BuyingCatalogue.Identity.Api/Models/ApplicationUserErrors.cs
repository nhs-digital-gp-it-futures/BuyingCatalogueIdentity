using NHSD.BuyingCatalogue.Identity.Common.Messages;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public static class ApplicationUserErrors
    {
        public static ErrorMessage FirstNameRequired()
        {
            return new ErrorMessage("FirstNameRequired", nameof(ApplicationUser.FirstName));
        }

        public static ErrorMessage FirstNameTooLong()
        {
            return new ErrorMessage("FirstNameTooLong", nameof(ApplicationUser.FirstName));
        }

        public static ErrorMessage LastNameRequired()
        {
            return new ErrorMessage("LastNameRequired", nameof(ApplicationUser.LastName));
        }

        public static ErrorMessage LastNameTooLong()
        {
            return new ErrorMessage("LastNameTooLong", nameof(ApplicationUser.LastName));
        }

        public static ErrorMessage PhoneNumberRequired()
        {
            return new ErrorMessage("PhoneNumberRequired", nameof(ApplicationUser.PhoneNumber));
        }

        public static ErrorMessage PhoneNumberTooLong()
        {
            return new ErrorMessage("PhoneNumberTooLong", nameof(ApplicationUser.PhoneNumber));
        }

        public static ErrorMessage EmailRequired()
        {
            return new ErrorMessage("EmailRequired", "EmailAddress");
        }

        public static ErrorMessage EmailTooLong()
        {
            return new ErrorMessage("EmailTooLong", "EmailAddress");
        }

        public static ErrorMessage EmailInvalidFormat()
        {
            return new ErrorMessage("EmailInvalidFormat", "EmailAddress");
        }
        
        public static ErrorMessage EmailAlreadyExists()
        {
            return new ErrorMessage("EmailAlreadyExists", "EmailAddress");
        }
    }
}
