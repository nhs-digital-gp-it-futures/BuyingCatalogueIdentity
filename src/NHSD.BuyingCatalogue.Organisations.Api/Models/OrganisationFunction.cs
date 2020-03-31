﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
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

        public override bool Equals(object obj)
        {
            if (!(obj is OrganisationFunction comparisonValue))
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Value.Equals(comparisonValue.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
