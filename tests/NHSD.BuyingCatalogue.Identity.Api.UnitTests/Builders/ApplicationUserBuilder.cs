using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ApplicationUserBuilder
    {
        private static readonly IDictionary<
            OrganisationFunction, 
            Func<ApplicationUserBuilder, ApplicationUser>> _applicationUserFactory 
            = new Dictionary<OrganisationFunction, Func<ApplicationUserBuilder, ApplicationUser>>
            {
                {
                    OrganisationFunction.Authority, builder => 
                        ApplicationUser.CreateAuthority(
                            builder._username, 
                            builder._firstName, 
                            builder._lastName, 
                            builder._phoneNumber, 
                            builder._emailAddress, 
                            builder._primaryOrganisationId)
                },
                {
                    OrganisationFunction.Buyer, builder => 
                        ApplicationUser.CreateBuyer(
                            builder._username, 
                            builder._firstName, 
                            builder._lastName, 
                            builder._phoneNumber, 
                            builder._emailAddress, 
                            builder._primaryOrganisationId)
                },
            };

        private string _userId;
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private string _emailAddress;
        private string _username;
        private Guid _primaryOrganisationId;
        private bool _disabled;
        private bool _catalogueAgreementSigned;
        private OrganisationFunction _organisationFunction;

        private ApplicationUserBuilder()
        {
            _userId = Guid.NewGuid().ToString();
            _firstName = "Bob";
            _lastName = "Smith";
            _phoneNumber = "0123456789";
            _emailAddress = "a.b@c.com";
            _username = _emailAddress;
            _primaryOrganisationId = Guid.NewGuid();
            _catalogueAgreementSigned = false;
            _organisationFunction = OrganisationFunction.Buyer;
        }

        internal static ApplicationUserBuilder Create() => new ApplicationUserBuilder();

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
            return this;
        }

        internal ApplicationUserBuilder WithUsername(string userName)
        {
            _username = userName;
            return this;
        }

        internal ApplicationUserBuilder WithPrimaryOrganisationId(Guid primaryOrganisationId)
        {
            _primaryOrganisationId = primaryOrganisationId;
            return this;
        }

        internal ApplicationUserBuilder WithOrganisationFunction(OrganisationFunction organisationFunction)
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

        internal ApplicationUser Build() => CreateUserByOrganisationFunction();

        private ApplicationUser CreateUserByOrganisationFunction()
        {
            if (!_applicationUserFactory.TryGetValue(_organisationFunction, out var factory))
            {
                throw new InvalidOperationException($"Unknown type of user '{_organisationFunction?.DisplayName}'");
            }

            var user = factory(this);
            user.Id = _userId;

            if (_disabled)
            {
                user.MarkAsDisabled();
            }

            if (_catalogueAgreementSigned)
            {
                user.MarkCatalogueAgreementAsSigned();
            }

            return user;
        }
    }
}
