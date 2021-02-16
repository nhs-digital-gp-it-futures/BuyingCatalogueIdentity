using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class AddressBuilder
    {
        private readonly string line2;
        private readonly string line3;
        private readonly string line4;
        private readonly string town;
        private readonly string county;
        private readonly string postcode;
        private readonly string country;

        private string line1;

        private AddressBuilder()
        {
            line1 = "Line1";
            line2 = "Line2";
            line3 = "Line3";
            line4 = "Line4";
            town = "Town";
            county = "County";
            postcode = "Postcode";
            country = "Country";
        }

        internal static AddressBuilder Create()
        {
            return new();
        }

        internal AddressBuilder WithLine1(string line)
        {
            line1 = line;
            return this;
        }

        internal Address Build()
        {
            return new()
            {
                Line1 = line1,
                Line2 = line2,
                Line3 = line3,
                Line4 = line4,
                Town = town,
                County = county,
                Postcode = postcode,
                Country = country,
            };
        }
    }
}
