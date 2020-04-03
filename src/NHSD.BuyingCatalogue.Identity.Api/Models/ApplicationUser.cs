using System;
using Microsoft.AspNetCore.Identity;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName => $"{FirstName} {LastName}";

        public Guid PrimaryOrganisationId { get; set; }

        public string OrganisationFunction { get; set; }
    }
}
