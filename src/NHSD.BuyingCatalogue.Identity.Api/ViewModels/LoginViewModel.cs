using System;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels
{
    public sealed class LoginViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }
    }
}
