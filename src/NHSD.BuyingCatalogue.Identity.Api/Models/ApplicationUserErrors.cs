using NHSD.BuyingCatalogue.Identity.Common.Messages;
﻿using NHSD.BuyingCatalogue.Identity.Common.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public static class ApplicationUserErrors
    {
        public static ErrorDetails FirstNameRequired()
        {
            return new ErrorDetails("FirstNameRequired", nameof(ApplicationUser.FirstName));
        }

        public static ErrorDetails FirstNameTooLong()
        {
            return new ErrorDetails("FirstNameTooLong", nameof(ApplicationUser.FirstName));
        }

        public static ErrorDetails LastNameRequired()
        {
            return new ErrorDetails("LastNameRequired", nameof(ApplicationUser.LastName));
        }

        public static ErrorDetails LastNameTooLong()
        {
            return new ErrorDetails("LastNameTooLong", nameof(ApplicationUser.LastName));
        }

        public static ErrorDetails PhoneNumberRequired()
        {
            return new ErrorDetails("PhoneNumberRequired", nameof(ApplicationUser.PhoneNumber));
        }

        public static ErrorDetails PhoneNumberTooLong()
        {
            return new ErrorDetails("PhoneNumberTooLong", nameof(ApplicationUser.PhoneNumber));
        }

        public static ErrorDetails EmailRequired()
        {
            return new ErrorDetails("EmailRequired", "EmailAddress");
        }

        public static ErrorDetails EmailTooLong()
        {
            return new ErrorDetails("EmailTooLong", "EmailAddress");
        }

        public static ErrorDetails EmailInvalidFormat()
        {
            return new ErrorDetails("EmailInvalidFormat", "EmailAddress");
        }
        
        public static ErrorDetails EmailAlreadyExists()
        {
            return new ErrorDetails("EmailAlreadyExists", "EmailAddress");
        }
    }
}
