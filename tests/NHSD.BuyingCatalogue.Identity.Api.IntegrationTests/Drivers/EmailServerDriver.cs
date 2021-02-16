using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers
{
    [Binding]
    internal sealed class EmailServerDriver
    {
        private readonly ScenarioContext context;
        private readonly Settings settings;

        public EmailServerDriver(ScenarioContext context, Settings settings)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        internal async Task<int> GetEmailCountAsync()
        {
            var emailList = await FindAllEmailsAsync();
            return emailList.Count();
        }

        internal async Task<IEnumerable<Email>> FindAllEmailsAsync()
        {
            var responseBody = await settings.SmtpServerApiBaseUrl
                .AppendPathSegment("email")
                .GetJsonListAsync();

            return responseBody.Select(m => new Email
            {
                PlainTextBody = m.text,
                HtmlBody = m.html,
                Subject = m.subject,
                From = m.from[0].address,
                To = m.to[0].address,
            });
        }

        internal async Task ClearAllEmailsAsync()
        {
            if (context.TryGetValue(ScenarioContextKeys.EmailSent, out bool _))
            {
                await settings.SmtpServerApiBaseUrl
                    .AppendPathSegments("email", "all")
                    .DeleteAsync();
            }
        }
    }
}
