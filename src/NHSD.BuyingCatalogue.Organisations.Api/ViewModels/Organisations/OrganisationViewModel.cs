using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class OrganisationViewModel
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public LocationViewModel Location { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
