using System;
using System.Text.Json;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    public static class StringExtensions
    {
        private static readonly DateTime StartTimeUtc = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime? ExtractCookieCreationDate(this string input)
        {
            if (input is null)
                return null;

            CookieData cookieData;
            try
            {
                cookieData = JsonSerializer.Deserialize<CookieData>(input);
            }
            catch (JsonException)
            {
                return null;
            }

            var milliseconds = cookieData?.CreationDate.GetValueOrDefault() ?? 0;
            if (milliseconds < 1)
                return null;

            return TimeZoneInfo.ConvertTimeFromUtc(
                StartTimeUtc + TimeSpan.FromMilliseconds(milliseconds),
                TimeZoneInfo.Local);
        }

        public static string ToCookieDataString(this DateTime dateTime)
        {
            return JsonSerializer.Serialize(
                new CookieData
                {
                    CookieValue = true,
                    CreationDate = (dateTime.ToUniversalTime() - StartTimeUtc).TotalMilliseconds,
                });
        }
    }
}
