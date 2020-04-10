using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public sealed class OrganisationFunction
    {
        public static readonly OrganisationFunction Authority = new OrganisationFunction(1, nameof(Authority));
        public static readonly OrganisationFunction Buyer = new OrganisationFunction(2, nameof(Buyer));

        private static readonly IEnumerable<OrganisationFunction> _values = new[] { Authority, Buyer };

        public int Value { get; }

        public string DisplayName { get; }

        private OrganisationFunction(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public static OrganisationFunction FromDisplayName(string displayName)
        {
            if (displayName is null)
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            return _values.SingleOrDefault(x => string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
        }

        private bool Equals(OrganisationFunction other)
        {
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is OrganisationFunction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
