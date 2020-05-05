﻿namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    internal sealed class PublicBrowseSettings : IPublicBrowseSettings
    {
        public string BaseAddress { get; set; }

        public string LoginPath { get; set; }
    }
}
