namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class OdsOrganisationModel
    {
        public string OdsCode { get; set; }

        public string OrganisationName { get; set; }

        public string PrimaryRoleId { get; set; }

        public AddressModel Address { get; set; }
    }
}
