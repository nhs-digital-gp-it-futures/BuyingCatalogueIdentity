namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels
{
    public sealed class ErrorViewModel
    {
        public string Message { get; set; }

        public bool ShowMessage => !string.IsNullOrWhiteSpace(Message);
    }
}
