namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users
{
    public sealed class CreateBuyerRequestModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }
    }
}
