using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }

        [Required(ErrorMessage = ErrorMessages.UsernameRequired)]
        public string Username { get; set; }

        internal static class ErrorMessages
        {
            internal const string PasswordRequired = "Enter your password";
            internal const string UsernameRequired = "Enter your email address";
        }
    }
}
