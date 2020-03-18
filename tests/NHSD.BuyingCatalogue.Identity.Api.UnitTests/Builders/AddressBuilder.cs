using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class AddressBuilder
    {
        private string _line1;
        private readonly string _line2;
        private readonly string _line3;
        private readonly string _line4;
        private readonly string _town;
        private readonly string _county;
        private readonly string _postcode;
        private readonly string _country;
        
        private AddressBuilder()
        {
            _line1 = "Line1";
            _line2 = "Line2";
            _line3 = "Line3";
            _line4 = "Line4";
            _town = "Town";
            _county = "County";
            _postcode = "Postcode";
            _country = "Country";
        }

        internal AddressBuilder WithLine1(string line1)
        {
            _line1 = line1;
            return this;
        }

        internal static AddressBuilder Create()
        {
            return new AddressBuilder();
        }

        internal Address Build()
        {
            return new Address()
            {
                Line1 = _line1,
                Line2 = _line2,
                Line3 = _line3,
                Line4 = _line4,
                Town = _town,
                County = _county,
                Postcode = _postcode,
                Country = _country
            };
        }
    }
}
