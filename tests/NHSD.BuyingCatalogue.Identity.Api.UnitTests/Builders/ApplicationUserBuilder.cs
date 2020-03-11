using System;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ApplicationUserBuilder
    {
        private string _userId;
        private string _userName;
        private string _email;
        private string _firstName;
        private string _lastName;
        private bool _emailConfirmed;
        private Guid _primaryOrganisationId;
        private string _organisationFunction;

        private ApplicationUserBuilder()
        {
            _userId = "TestUserId";
            _userName = "TestUserName";
            _email = null;
            _emailConfirmed = false;
            _firstName = null;
            _lastName = null;
            _primaryOrganisationId = Guid.NewGuid();
            _organisationFunction = null;
        }

        internal static ApplicationUserBuilder Create()
        {
            return new ApplicationUserBuilder();
        }

        internal ApplicationUserBuilder WithId(string userId)
        {
            _userId = userId;
            return this;
        }

        internal ApplicationUserBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        internal ApplicationUserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        internal ApplicationUserBuilder WithEmailConfirmation(bool emailConfirmed)
        {
            _emailConfirmed = emailConfirmed;
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

        internal ApplicationUser Build()
        {
            return new ApplicationUser
            {
                Id = _userId,
                UserName = _userName,
                Email = _email,
                EmailConfirmed = _emailConfirmed,
                FirstName = _firstName,
                LastName = _lastName,
                PrimaryOrganisationId = _primaryOrganisationId,
                OrganisationFunction = _organisationFunction
            };
        }
    }
}
