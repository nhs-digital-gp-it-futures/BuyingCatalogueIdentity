using System;

namespace NHSD.BuyingCatalogue.Identity.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimAsync(this string input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return input.Replace("async", "", StringComparison.OrdinalIgnoreCase);
        }
    }
}
