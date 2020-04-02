using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users
{
    public sealed class GetUser
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public bool Disabled { get; set; }

        public Guid PrimaryOrganisationId { get; set; }
    }
}
