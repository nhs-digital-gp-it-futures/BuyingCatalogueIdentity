namespace NHSD.BuyingCatalogue.Organisations.Api.Settings
{
    internal sealed class OdsSettings
    {
        public string ApiBaseUrl { get; set; }
        public string[] BuyerOrganisationRoleIds { get; set; }
        public string GpPracticeRoleId { get; set; }
        public int GetChildOrganisationSearchLimit { get; set; }
    }
}
