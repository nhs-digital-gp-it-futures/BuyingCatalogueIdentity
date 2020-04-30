using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
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

        public EmailServerDriver(ScenarioContext context, Settings settings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        internal async Task<int> GetEmailCountAsync()
        {
            var emailList = await FindAllEmailsAsync();
            return emailList.Count();
        }

        internal async Task<IEnumerable<Email>> FindAllEmailsAsync()
        {
            var responseBody = await _settings.SmtpServerApiBaseUrl
                .AppendPathSegment("email")
                .GetJsonListAsync();

            return responseBody.Select(x => new Email
            {
                PlainTextBody = x.text,
                HtmlBody = x.html,
                Subject = x.subject,
                From = x.from[0].address,
                To = x.to[0].address
            });
        }

        internal async Task ClearAllEmailsAsync()
        {
            if (_context.TryGetValue(ScenarioContextKeys.EmailSent, out bool _))
            {
                await _settings.SmtpServerApiBaseUrl
                    .AppendPathSegments("email", "all")
                    .DeleteAsync();
            }
        }
    }
}
