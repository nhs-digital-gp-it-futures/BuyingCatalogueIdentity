using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [StringLength(257, MinimumLength = 10, ErrorMessage = ErrorMessages.PasswordConditionsNotMet)]
        [PasswordValidation(ErrorMessage = ErrorMessages.PasswordConditionsNotMet)]
        [DisplayName("Enter a password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = ErrorMessages.PasswordMismatch)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        internal static class ErrorMessages
        {
            internal const string PasswordConditionsNotMet = "Password does not meet the Password Policy";
            internal const string PasswordMismatch = "Password does not match";
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        internal class PasswordValidation : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (!(value is string))
                {
                    return false;
                }
                var specialCharacters = "!@#$%^&*";

                var password = value as string;

                var validationRules = new List<Func<bool>>
                {
                    () => password.Any(char.IsLower),
                    () => password.Any(char.IsUpper),
                    () => password.Any(char.IsDigit),
                    () => password.Contains(specialCharacters, StringComparison.InvariantCulture)
                };

                return validationRules.Count(x => x()) >= 3;
            }
        }
    }
}
