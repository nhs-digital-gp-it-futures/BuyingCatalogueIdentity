using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class CookieExpirationSettings
    {
        public TimeSpan ConsentExpiration { get; set; }

        public TimeSpan ExpireTimeSpan { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
