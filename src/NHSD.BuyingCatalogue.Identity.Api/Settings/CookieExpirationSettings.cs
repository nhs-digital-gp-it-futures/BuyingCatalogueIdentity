﻿using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class CookieExpirationSettings
    {
        public DateTime? BuyingCatalogueCookiePolicyDate { get; set; }

        public TimeSpan ConsentExpiration { get; set; }

        public TimeSpan ExpireTimeSpan { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
