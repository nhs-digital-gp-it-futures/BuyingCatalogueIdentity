using System;
using Microsoft.AspNetCore.Identity;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public Guid PrimaryOrganisationId { get; set; }

        public string OrganisationFunction { get; set; }
    }
}
