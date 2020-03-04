using System;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Organisations
{
    public sealed class OrganisationViewModel
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }
    }
}
