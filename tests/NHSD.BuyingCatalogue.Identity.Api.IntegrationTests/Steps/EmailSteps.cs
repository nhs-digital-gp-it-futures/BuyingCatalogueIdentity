using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
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

        [Then(@"an email containing the following information is sent")]
        public void ThenEmailIsSent(Table emailTable)
        {
            var data = emailTable.CreateInstance<EmailTable>();

            var message = CreateMessage(data);
            _context["messageId"] = message.MessageId;

            Send(message);
        }

        [Then(@"the sent email contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedEmail = table.CreateInstance<EmailTable>();
            var actualEmail = await GetActualEmail();

            actualEmail.Body.Should().Contain(expectedEmail.Body);
            expectedEmail.Should().BeEquivalentTo(actualEmail, options => options.Excluding(e => e.Body).Excluding(e => e.MessageId));
        }

        [Then(@"email inbox contains no new emails")]
        public async Task InboxContainsNoNewEmails()
        {
            var emails = await GetAllEmailsFromSmtpServer();
            var mostRecentEmailId = _context.TryGetValue("messageId", out string messageId) ? messageId : string.Empty;
            emails.FirstOrDefault(e => e.MessageId.Equals(mostRecentEmailId, StringComparison.OrdinalIgnoreCase))
                .Should().BeNull();
        }

        private static MimeMessage CreateMessage(EmailTable email)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(email.From));
            message.To.Add(new MailboxAddress(email.To));
            message.Subject = email.Subject;

            BodyBuilder bodyBuilder = new BodyBuilder { TextBody = email.Body };
            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        private void Send(MimeMessage message)
        {
            using SmtpClient client = new SmtpClient();
            client.Connect(_settings.Smtp.Host, _settings.Smtp.Port, SecureSocketOptions.None);
            client.Send(message);
        }

        private async Task<EmailTable> GetActualEmail()
        {
            var allEmails = await GetAllEmailsFromSmtpServer();
            return allEmails.First(e => e.MessageId.Equals(_context["messageId"].ToString(), StringComparison.OrdinalIgnoreCase));
        }

        private async Task<IEnumerable<EmailTable>> GetAllEmailsFromSmtpServer()
        {
            using var client = new HttpClient();
            _response.Result = await client.GetAsync(new Uri($"{_settings.Smtp.ApiBaseUrl}/email"));
            _response.Result.Should().NotBeNull();

            return (await _response.ReadBody()).Select(x => new EmailTable()
            {
                Body = x.SelectToken("text").ToString().Trim(),
                Subject = x.SelectToken("subject").ToString(),
                From = x.SelectToken("from").First().SelectToken("address").ToString(),
                To = x.SelectToken("to").First().SelectToken("address").ToString(),
                MessageId = x.SelectToken("messageId").ToString()
            });
        }

        [AfterTestRun]
        public static async Task CleanUp()
        {
            using var client = new HttpClient();
            await client.DeleteAsync(new Uri("http://localhost:1080/email/all"));
        }

        private class EmailTable
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Subject { get; set; }

            public string Body { get; set; }

            public string MessageId { get; set; }
        }
    }
}
