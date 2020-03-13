﻿using System.Security;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    public class CertificateSettings
    {
        public bool UseDeveloperCredentials { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
    }
}