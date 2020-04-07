using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers
{
    [Binding]
    public sealed class EmailServerDriver
    {
        private readonly ScenarioContext _context;
        private readonly Settings _settings;
        private readonly Uri _listAllEmailsUri;

        public EmailServerDriver(ScenarioContext context, Settings settings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _listAllEmailsUri = new Uri($"{_settings.SmtpServerApiBaseUrl}/email");
        }

        internal async Task<int> GetEmailCountAsync()
        {
            var emailList = await FindAllEmailsAsync();
            return emailList.Count();
        }

        internal async Task<IEnumerable<Email>> FindAllEmailsAsync()
        {
            Response response = new Response();

            using var client = new HttpClient();
            response.Result = await client.GetAsync(_listAllEmailsUri);

            var responseContent = await response.ReadBodyAsJsonAsync();

            return responseContent.Select(x => new Email
            {
                PlainTextBody = x.SelectToken("text").ToString().Trim(),
                HtmlBody = x.SelectToken("html").ToString().Trim(),
                Subject = x.SelectToken("subject").ToString(),
                From = x.SelectToken("from").First().SelectToken("address").ToString(),
                To = x.SelectToken("to").First().SelectToken("address").ToString(),
            });
        }

        internal async Task ClearAllEmailsAsync()
        {
            if (_context.TryGetValue(ScenarioContextKeys.EmailSent, out bool _))
            {
                using var client = new HttpClient();
                await client.DeleteAsync(new Uri($"{_settings.SmtpServerApiBaseUrl}/email/all"));
            }
        }
    }
}
