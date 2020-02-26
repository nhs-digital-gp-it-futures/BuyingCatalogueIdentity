using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels
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
