﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Common.Messages;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Validators
{
    public class ApplicationUserValidator : IApplicationUserValidator
    {
        private const int MaximumFirstNameLength = 100;
        private const int MaximumLastNameLength = 100;
        private const int MaximumPhoneNumberLength = 35;
        private const int MaximumEmailLength = 256;

        private static readonly EmailAddressAttribute _emailAddressAttribute = new EmailAddressAttribute();

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

            ValidateName(user.FirstName, user.LastName, errors);
            ValidatePhoneNumber(user.PhoneNumber, errors);

            await ValidateEmailAsync(user.Email, errors);

            return errors.Any() ? Result.Failure(errors) : Result.Success();
        }

        private static void ValidateName( 
            string firstName, 
            string lastName,
            List<ErrorMessage> errors)
        {
            ValidateFirstName(firstName, errors);
            ValidateLastName(lastName, errors);
        }

        private static void ValidateFirstName(string firstName, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(ApplicationUserErrors.FirstNameRequired());
                return;
            }

            if (firstName.Length > MaximumFirstNameLength)
                errors.Add(ApplicationUserErrors.FirstNameTooLong());
        }

        private static void ValidateLastName(string lastName, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(ApplicationUserErrors.LastNameRequired());
                return;
            }

            if (lastName.Length > MaximumLastNameLength)
                errors.Add(ApplicationUserErrors.LastNameTooLong());
        }

        private static void ValidatePhoneNumber(string phoneNumber, List<ErrorMessage> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(ApplicationUserErrors.PhoneNumberRequired());
                return;
            }

            if (phoneNumber.Length > MaximumPhoneNumberLength)
                errors.Add(ApplicationUserErrors.PhoneNumberTooLong());
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
            else if (!_emailAddressAttribute.IsValid(email))
            {
                errors.Add(ApplicationUserErrors.EmailInvalidFormat());
            }
            else
            {
                var user = await _usersRepository.GetByEmailAsync(email);
                if (user is object &&
                    string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(ApplicationUserErrors.EmailAlreadyExists());
                }
            }
        }
    }
}
