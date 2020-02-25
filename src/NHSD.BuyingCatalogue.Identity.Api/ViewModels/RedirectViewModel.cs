using System;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels
{
    public class RedirectViewModel
    {
        public RedirectViewModel(Uri redirectUrl) => RedirectUrl = redirectUrl;

        public Uri RedirectUrl { get; set; }
    }
}
