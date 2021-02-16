using System;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    public sealed class CreateBuyerRequestBuilder
    {
        private Guid primaryOrganisationId;
        private string firstName;
        private string lastName;
        private string phoneNumber;
        private string emailAddress;

        public CreateBuyerRequestBuilder()
        {
            primaryOrganisationId = Guid.NewGuid();
            firstName = "Test";
            lastName = "Smith";
            phoneNumber = "0123456789";
            emailAddress = "a.b@c.com";
        }

        public static CreateBuyerRequestBuilder Create()
        {
            return new();
        }

        public CreateBuyerRequestBuilder WithPrimaryOrganisationId(Guid id)
        {
            primaryOrganisationId = id;
            return this;
        }

        public CreateBuyerRequestBuilder WithFirstName(string name)
        {
            firstName = name;
            return this;
        }

        public CreateBuyerRequestBuilder WithLastName(string name)
        {
            lastName = name;
            return this;
        }

        public CreateBuyerRequestBuilder WithPhoneNumber(string number)
        {
            phoneNumber = number;
            return this;
        }

        public CreateBuyerRequestBuilder WithEmailAddress(string address)
        {
            emailAddress = address;
            return this;
        }

        public CreateBuyerRequest Build()
        {
            return new(primaryOrganisationId, firstName, lastName, phoneNumber, emailAddress);
        }
    }
}
