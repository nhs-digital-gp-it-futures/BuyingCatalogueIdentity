using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class OdsOrganisation
    {
        public string OdsCode { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public string PrimaryRoleId { get; set; }

        public Address Address { get; set; }

        public bool IsActive { get; set; }

        public bool IsBuyerOrganisation { get; set; }
    }
}
