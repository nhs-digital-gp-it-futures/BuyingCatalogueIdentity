using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [DisplayName("Enter a password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = ErrorMessages.PasswordMismatch)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        private static class ErrorMessages
        {
            internal const string PasswordMismatch = "Passwords do not match";
        }
    }
}
