using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class OdsViewModel
    {
        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public AddressViewModel Address { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
