using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = "Enter your password")]
        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        public string Username { get; set; }
    }
}
