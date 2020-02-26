using System;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels
{
    public sealed class RedirectViewModel
    {
        public RedirectViewModel(Uri redirectUrl) => RedirectUrl = redirectUrl;

        public Uri RedirectUrl { get; set; }
    }
}
