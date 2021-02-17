using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public sealed class CreateOrganisationRequest
    {
        public CreateOrganisationRequest(
            string organisationName,
            string odsCode,
            string primaryRoleId,
            bool catalogueAgreementSigned,
            Address address)
        {
            OrganisationName = organisationName;
            OdsCode = odsCode;
            PrimaryRoleId = primaryRoleId;
            CatalogueAgreementSigned = catalogueAgreementSigned;
            Address = address;
        }

        public string OrganisationName { get; }

        public string OdsCode { get; }

        public string PrimaryRoleId { get; }

        public bool CatalogueAgreementSigned { get; }

        public Address Address { get; }
    }
}
