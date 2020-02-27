using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required]
        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
