namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data
{
    internal sealed class Email
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string PlainTextBody { get; set; }

        public string HtmlBody { get; set; }
    }
}
