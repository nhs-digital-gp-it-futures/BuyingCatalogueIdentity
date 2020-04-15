using System;
using NHSD.BuyingCatalogue.Identity.Api.Services.CreateBuyer;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    public sealed class CreateBuyerRequestBuilder
    {
        private Guid _primaryOrganisationId;
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private string _emailAddress;

        public CreateBuyerRequestBuilder()
        {
            _primaryOrganisationId = Guid.NewGuid();
            _firstName = "Test";
            _lastName = "Smith";
            _phoneNumber = "0123456789";
            _emailAddress = "a.b@c.com";
        }

        public static CreateBuyerRequestBuilder Create()
        {
            return new CreateBuyerRequestBuilder();
        }

        public CreateBuyerRequestBuilder WithPrimaryOrganisationId(Guid primaryOrganisationId)
        {
            _primaryOrganisationId = primaryOrganisationId;
            return this;
        }

        public CreateBuyerRequestBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public CreateBuyerRequestBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public CreateBuyerRequestBuilder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }

        public CreateBuyerRequestBuilder WithEmailAddress(string emailAddress)
        {
            _emailAddress = emailAddress;
            return this;
        }

        public CreateBuyerRequest Build()
        {
            return new CreateBuyerRequest(_primaryOrganisationId, _firstName, _lastName, _phoneNumber, _emailAddress);
        }
    }
}
