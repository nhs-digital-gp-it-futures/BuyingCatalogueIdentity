using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.PasswordReset
{
    public sealed class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessages.EmailAddressRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalid)]
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        internal static class ErrorMessages
        {
            internal const string EmailAddressRequired = "Enter an email address";
            internal const string EmailAddressInvalid = "Enter an email address in the correct format, like name@example.com";
        }
    }
}
