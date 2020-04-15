using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Infrastructure
{
    internal sealed class PasswordValidator : IPasswordValidator<ApplicationUser>
    {
        internal const string InvalidPasswordCode = "InvalidPassword";
        internal const string PasswordConditionsNotMet = "The password you’ve entered does not meet the criteria";

        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            const string specialCharacters = "!@#$%^&*";

            if (password.Length < 10)
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = InvalidPasswordCode,
                    Description = PasswordConditionsNotMet
                }));

            var validationRules = new List<Func<bool>>
            {
                () => password.Any(char.IsLower),
                () => password.Any(char.IsUpper),
                () => password.Any(char.IsDigit),
                () => password.Any(x => specialCharacters.Contains(x, StringComparison.InvariantCultureIgnoreCase))
            };

            if (validationRules.Count(x => x()) < 3)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = InvalidPasswordCode,
                    Description = PasswordConditionsNotMet
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
