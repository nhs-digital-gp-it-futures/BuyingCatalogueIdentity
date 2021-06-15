using System;
using System.Text.Json;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    public static class StringExtensions
    {
        private static readonly DateTime StartTimeUtc = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ExtractCookieCreationDate(this string input)
        {
            // Will force banner to be displayed and cookie rewritten with correct data when banner is dismissed again
            static DateTime ForceBannerDisplay() => DateTime.MinValue;

            if (input is null)
                return ForceBannerDisplay();

            CookieData cookieData;
            try
            {
                cookieData = JsonSerializer.Deserialize<CookieData>(
                    input,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return ForceBannerDisplay();
            }

            var milliseconds = cookieData?.CreationDate.GetValueOrDefault() ?? 0;
            if (milliseconds < 1)
                return ForceBannerDisplay();

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
