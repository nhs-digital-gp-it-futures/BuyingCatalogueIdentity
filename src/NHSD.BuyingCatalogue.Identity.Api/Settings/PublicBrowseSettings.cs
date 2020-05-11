﻿using System;
using Microsoft.AspNetCore.Http;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public sealed class PublicBrowseSettings
    {
        public string BaseAddress { get; set; }

        public PathString LoginPath { get; set; }

        public Uri LoginAddress
        {
            get
            {
                return new Uri(BaseAddress + LoginPath);
            }
        }
    }
}