﻿﻿using System;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class ApplicationUserBuilder
    {
        private string _userId;
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private string _emailAddress;
        private string _normalizedEmailAddress;
        private Guid _primaryOrganisationId;
        private string _organisationFunction;
        private bool _disabled;
        private bool _catalogueAgreementSigned;

        private ApplicationUserBuilder()
        {
            _userId = Guid.NewGuid().ToString();
            _firstName = "Bob";
            _lastName = "Smith";
            _phoneNumber = "0123456789";
            _emailAddress = "a.b@c.com";
            _normalizedEmailAddress = _emailAddress.ToUpperInvariant();
            _primaryOrganisationId = Guid.NewGuid();
            _organisationFunction = "Buyer";
            _catalogueAgreementSigned = false;
        }

        internal static ApplicationUserBuilder Create()
        {
            return new ApplicationUserBuilder();
        }

        internal ApplicationUserBuilder WithUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        internal ApplicationUserBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        internal ApplicationUserBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        internal ApplicationUserBuilder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }

        internal ApplicationUserBuilder WithEmailAddress(string emailAddress)
        {
            _emailAddress = emailAddress;
            _normalizedEmailAddress = _emailAddress?.ToUpperInvariant();

            return this;
        }

        internal ApplicationUserBuilder WithPrimaryOrganisationId(Guid primaryOrganisationId)
        {
            _primaryOrganisationId = primaryOrganisationId;
            return this;
        }

        internal ApplicationUserBuilder WithOrganisationFunction(string organisationFunction)
        {
            _organisationFunction = organisationFunction;
            return this;
        }

        internal ApplicationUserBuilder WithDisabled(bool disabled)
        {
            _disabled = disabled;
            return this;
        }

        internal ApplicationUserBuilder WithCatalogueAgreementSigned(bool catalogueAgreementSigned)
        {
            _catalogueAgreementSigned = catalogueAgreementSigned;
            return this;
        }

        internal ApplicationUser Build()
        {
            return new ApplicationUser
            {
                Id = _userId, 
                FirstName = _firstName, 
                LastName = _lastName, 
                PhoneNumber = _phoneNumber,
                Email = _emailAddress, 
                NormalizedEmail = _normalizedEmailAddress,
                PrimaryOrganisationId = _primaryOrganisationId, 
                OrganisationFunction = _organisationFunction,
                Disabled = _disabled,
                CatalogueAgreementSigned = _catalogueAgreementSigned
            };
        }
    }
}