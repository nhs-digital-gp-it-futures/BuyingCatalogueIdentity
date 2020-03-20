using System;
using Microsoft.AspNetCore.Identity;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid PrimaryOrganisationId { get; set; }

        public string OrganisationFunction { get; set; }

        public bool Disabled { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
