using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    internal sealed class CookieExpirationSettings
    {
        public TimeSpan ExpireTimeSpan { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
