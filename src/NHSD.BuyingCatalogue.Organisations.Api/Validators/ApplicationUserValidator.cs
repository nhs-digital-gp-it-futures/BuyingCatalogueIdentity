using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Errors;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Models.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.Validators
{
    public class ApplicationUserValidator : IApplicationUserValidator
    {
        private const int MaximumFirstNameLength = 50;
        private const int MaximumLastNameLength = 50;
        private const int MaximumEmailLength = 256;

        private readonly IUsersRepository _usersRepository;

        public ApplicationUserValidator(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<Result> ValidateAsync(ApplicationUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            List<ErrorMessage> errors = new List<ErrorMessage>();

            await ValidateNameAsync(user.FirstName, user.LastName, errors);
            await ValidatePhoneNumberAsync(user.PhoneNumber, errors);
            await ValidateEmailAsync(user.Email, errors);

            return errors.Any() ? Result.Failure(errors) : Result.Success();
        }

        private static async Task ValidateNameAsync( 
            string firstName, 
            string lastName,
            List<ErrorMessage> errors)
        {
            await ValidateFirstNameAsync(firstName, errors);
            await ValidateLastNameAsync(lastName, errors);
        }

        private static Task ValidateFirstNameAsync(string firstName, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(ApplicationUserErrors.FirstNameRequired());
                return Task.CompletedTask;
            }

            if (firstName.Length > MaximumFirstNameLength)
                errors.Add(ApplicationUserErrors.FirstNameTooLong());

            return Task.CompletedTask;
        }

        private static Task ValidateLastNameAsync(string lastName, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(ApplicationUserErrors.LastNameRequired());
                return Task.CompletedTask;
            }

            if (lastName.Length > MaximumLastNameLength)
                errors.Add(ApplicationUserErrors.LastNameTooLong());
            

            return Task.CompletedTask;
        }

        private static Task ValidatePhoneNumberAsync(string phoneNumber, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(ApplicationUserErrors.PhoneNumberRequired());
            }

            return Task.CompletedTask;
        }

        private async Task ValidateEmailAsync(string email, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(ApplicationUserErrors.EmailRequired());
                return;
            }

            if (email.Length > MaximumEmailLength)
            {
                errors.Add(ApplicationUserErrors.EmailTooLong());
            }
            else if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
            {
                errors.Add(ApplicationUserErrors.EmailInvalidFormat());
            }
            else
            {
                var user = await _usersRepository.FindUserByEmailAsync(email);
                if (user is object &&
                    string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(ApplicationUserErrors.EmailAlreadyExists());
                }
            }
        }
    }
}
