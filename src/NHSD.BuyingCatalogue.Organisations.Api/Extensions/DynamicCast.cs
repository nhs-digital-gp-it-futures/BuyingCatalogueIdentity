using System;
using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Organisations.Api.Extensions
{
    public static class DynamicCast
    {
        public static T GetPropertyOrDefault<T>(dynamic dynamicObject, string propertyName)
        {
            if (dynamicObject is null)
                throw new ArgumentNullException(nameof(dynamicObject));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} can not be null or empty string.", nameof(propertyName));

            if (((IDictionary<string, object>)dynamicObject).TryGetValue(propertyName, out object value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
