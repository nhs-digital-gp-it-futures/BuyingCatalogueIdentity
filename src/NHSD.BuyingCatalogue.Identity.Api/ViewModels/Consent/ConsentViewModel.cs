using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Consent
{
    public sealed class ConsentViewModel
    {
        // TODO: 6329 – confirm correct error message
        [Required]
        [RegularExpression(
            "(?:True|true)",
            ErrorMessage = "Accept End User Agreement")]
        [DisplayName("Accept End User Agreement")]
        public bool AgreeWithTerms { get; set; }

        public Uri ReturnUrl { get; set; }
    }
}
