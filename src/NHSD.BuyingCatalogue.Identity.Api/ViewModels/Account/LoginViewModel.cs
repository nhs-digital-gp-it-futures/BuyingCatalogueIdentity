using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = ErrorMessages.EmailAddressRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalid)]
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }

        public string DisabledError { get; set; }

        internal static class ErrorMessages
        {
            internal const string EmailAddressRequired = "Enter your email address";
            internal const string EmailAddressInvalid = "Enter a valid email address";
            internal const string PasswordRequired = "Enter your password";
        }
    }
}
