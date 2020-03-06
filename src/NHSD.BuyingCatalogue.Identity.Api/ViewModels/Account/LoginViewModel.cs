using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }

        [Required(ErrorMessage = ErrorMessages.EmailAddressRequired)]
        [DisplayName("Email address")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        internal static class ErrorMessages
        {
            internal const string PasswordRequired = "Enter your password";
            internal const string EmailAddressRequired = "Enter your email address";
        }
    }
}
