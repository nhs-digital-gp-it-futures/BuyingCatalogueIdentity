using System;
using System.Collections.Generic;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ApplicationUserBuilder
    {
        private static readonly IDictionary<
            OrganisationFunction,
            Func<ApplicationUserBuilder, ApplicationUser>> ApplicationUserFactory =
            new Dictionary<OrganisationFunction, Func<ApplicationUserBuilder, ApplicationUser>>
            {
                {
                    OrganisationFunction.Authority, builder =>
                        ApplicationUser.CreateAuthority(
                            builder.username,
                            builder.firstName,
                            builder.lastName,
                            builder.phoneNumber,
                            builder.emailAddress,
                            builder.primaryOrganisationId)
                },
                {
                    OrganisationFunction.Buyer, builder =>
                        ApplicationUser.CreateBuyer(
                            builder.username,
                            builder.firstName,
                            builder.lastName,
                            builder.phoneNumber,
                            builder.emailAddress,
                            builder.primaryOrganisationId)
                },
            };

        private string userId;
        private string firstName;
        private string lastName;
        private string phoneNumber;
        private string emailAddress;
        private string username;
        private Guid primaryOrganisationId;
        private bool disabled;
        private bool catalogueAgreementSigned;
        private OrganisationFunction organisationFunction;

        private ApplicationUserBuilder()
        {
            userId = Guid.NewGuid().ToString();
            firstName = "Bob";
            lastName = "Smith";
            phoneNumber = "0123456789";
            emailAddress = "a.b@c.com";
            username = emailAddress;
            primaryOrganisationId = Guid.NewGuid();
            catalogueAgreementSigned = false;
            organisationFunction = OrganisationFunction.Buyer;
        }

        internal static ApplicationUserBuilder Create() => new();

        internal ApplicationUserBuilder WithUserId(string id)
        {
            userId = id;
            return this;
        }

        internal ApplicationUserBuilder WithFirstName(string name)
        {
            firstName = name;
            return this;
        }

        internal ApplicationUserBuilder WithLastName(string name)
        {
            lastName = name;
            return this;
        }

        internal ApplicationUserBuilder WithPhoneNumber(string number)
        {
            phoneNumber = number;
            return this;
        }

        internal ApplicationUserBuilder WithEmailAddress(string address)
        {
            emailAddress = address;
            return this;
        }

        internal ApplicationUserBuilder WithUsername(string name)
        {
            username = name;
            return this;
        }

        internal ApplicationUserBuilder WithPrimaryOrganisationId(Guid id)
        {
            primaryOrganisationId = id;
            return this;
        }

        internal ApplicationUserBuilder WithOrganisationFunction(OrganisationFunction function)
        {
            organisationFunction = function;
            return this;
        }

        internal ApplicationUserBuilder WithDisabled(bool isDisabled)
        {
            disabled = isDisabled;
            return this;
        }

        internal ApplicationUserBuilder WithCatalogueAgreementSigned(bool agreementSigned)
        {
            catalogueAgreementSigned = agreementSigned;
            return this;
        }

        internal ApplicationUser Build() => CreateUserByOrganisationFunction();

        private ApplicationUser CreateUserByOrganisationFunction()
        {
            if (!ApplicationUserFactory.TryGetValue(organisationFunction, out var factory))
            {
                throw new InvalidOperationException($"Unknown type of user '{organisationFunction?.DisplayName}'");
            }

            var user = factory(this);
            user.Id = userId;

            if (disabled)
            {
                user.MarkAsDisabled();
            }

            if (catalogueAgreementSigned)
            {
                user.MarkCatalogueAgreementAsSigned();
            }

            return user;
        }
    }
}
