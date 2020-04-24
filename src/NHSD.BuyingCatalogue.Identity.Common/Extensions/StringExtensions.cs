using System;

namespace NHSD.BuyingCatalogue.Identity.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimAsync(this string input)
        {
            return TrimSuffix(input, "async", StringComparison.OrdinalIgnoreCase);
        }

        public static string TrimController(this string input)
        {
            return TrimSuffix(input, "Controller", StringComparison.Ordinal);
        }

        private static string TrimSuffix(string input, string suffix, StringComparison comparison)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return input.EndsWith(suffix, comparison)
                ? input.Substring(0, input.Length - suffix.Length)
                : input;
        }
    }
}
