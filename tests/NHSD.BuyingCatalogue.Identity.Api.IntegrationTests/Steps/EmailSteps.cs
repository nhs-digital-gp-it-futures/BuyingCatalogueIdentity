using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly Response _response;

        private readonly ScenarioContext _context;

        private readonly Settings _settings;

        public EmailSteps(Response response, ScenarioContext context, Settings settings)
        {
            _response = response;
            _context = context;
            _settings = settings;
        }

        [Then(@"the email sent contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedEmail = table.CreateInstance<EmailTable>();
            var actualEmail = await GetActualEmail();

            actualEmail.PlainTextBody.Should().Contain(expectedEmail.ResetPasswordLink);
            actualEmail.HtmlBody.Should().Contain(expectedEmail.ResetPasswordLink);
            actualEmail.Should().BeEquivalentTo(expectedEmail, options => options.Excluding(e => e.ResetPasswordLink));
        }

        [Then(@"no email is sent")]
        public async Task EmailIsNotSent()
        {
            var emails = await GetAllEmailsFromSmtpServer();
            emails.Should().BeNullOrEmpty();
        }

        [Then(@"only one email is sent")]
        public async Task OnlyOneEmailIsSent()
        {
            var emails = await GetAllEmailsFromSmtpServer();
            emails.Count().Should().Be(1);
        }

        private async Task<Email> GetActualEmail()
        {
            var allEmails = await GetAllEmailsFromSmtpServer();
            return allEmails.First();
        }

        private async Task<IEnumerable<Email>> GetAllEmailsFromSmtpServer()
        {
            using var client = new HttpClient();
            _response.Result = await client.GetAsync(new Uri($"{_settings.SmtpServerApiBaseUrl}/email"));
            _response.Result.Should().NotBeNull();

            return (await _response.ReadBody()).Select(x => new Email
            {
                PlainTextBody = x.SelectToken("text").ToString().Trim(),
                HtmlBody = x.SelectToken("html").ToString().Trim(),
                Subject = x.SelectToken("subject").ToString(),
                From = x.SelectToken("from").First().SelectToken("address").ToString(),
                To = x.SelectToken("to").First().SelectToken("address").ToString(),
            });
        }

        [AfterScenario]
        public async Task CleanUp()
        {
            if (_context.TryGetValue("emailSent", out bool _))
            {
                using var client = new HttpClient();
                await client.DeleteAsync(new Uri($"{_settings.SmtpServerApiBaseUrl}/email/all"));
            }
        }

        private class EmailTable
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Subject { get; set; }

            public string ResetPasswordLink { get; set; }
        }

        private class Email
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Subject { get; set; }

            public string PlainTextBody { get; set; }

            public string HtmlBody { get; set; }
        }
    }
}
