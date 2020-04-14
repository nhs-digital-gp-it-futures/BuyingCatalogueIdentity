namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class OdsOrganisationViewModel
    {
        public string OdsCode { get; set; }

        public string OrganisationName { get; set; }

        public string PrimaryRoleId { get; set; }

        public AddressViewModel Address { get; set; }
    }
}
