using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Consent
{
    public sealed class ConsentViewModel
    {
        // TODO: confirm correct error message
        [Required]
        [RegularExpression(
            "(?:True|true)",
            ErrorMessage = "You must agree to the terms and conditions before continuing.")]
        [DisplayName("I agree with the terms")]
        public bool AgreeWithTerms { get; set; }

        public Uri ReturnUrl { get; set; }
    }
}
