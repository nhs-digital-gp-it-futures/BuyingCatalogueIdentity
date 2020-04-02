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
        [PasswordValidation(ErrorMessage = ErrorMessages.PasswordConditionsNotMet)]
        [DisplayName("Enter a password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = ErrorMessages.PasswordMismatch)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        private static class ErrorMessages
        {
            internal const string PasswordConditionsNotMet = "The password you’ve entered does not meet the criteria";
            internal const string PasswordMismatch = "Passwords do not match";
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        private class PasswordValidation : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (!(value is string))
                {
                    return false;
                }

                const string specialCharacters = "!@#$%^&*";

                var password = value.ToString();

                if (password.Length < 10)
                    return false;

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
