namespace NHSD.BuyingCatalogue.Identity.Api.SampleMvcClient.ViewModels
{
    public sealed class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
