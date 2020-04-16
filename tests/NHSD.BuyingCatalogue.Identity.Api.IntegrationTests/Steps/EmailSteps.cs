using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly EmailServerDriver _emailServerDriver;
        private readonly Settings _settings;

        public EmailSteps(EmailServerDriver emailServerDriver, Settings settings)
        {
            _emailServerDriver = emailServerDriver ?? throw new ArgumentNullException(nameof(emailServerDriver));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Then(@"the email sent contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedEmail = table.CreateInstance<EmailTable>();
            var actualEmail = (await _emailServerDriver.FindAllEmailsAsync()).First();

            actualEmail.PlainTextBody.Should().MatchRegex(PasswordResetUrlRegex(expectedEmail.To));
            actualEmail.HtmlBody.Should().MatchRegex(PasswordResetUrlRegex(expectedEmail.To));
            actualEmail.Should().BeEquivalentTo(expectedEmail);
        }

        [Then(@"no email is sent")]
        public async Task EmailIsNotSent()
        {
            var emails = await _emailServerDriver.FindAllEmailsAsync();
            emails.Should().BeNullOrEmpty();
        }

        [Then(@"only one email is sent")]
        public async Task OnlyOneEmailIsSent()
        {
            var actualCount = await _emailServerDriver.GetEmailCountAsync();
            actualCount.Should().Be(1);
        }

        private string PasswordResetUrlRegex(string expectedEmailAddress) =>
            $@"(?:\s|<a href=""){_settings.IdentityApiBaseUrl}/Account/ResetPassword\?Token=(?:\S+)&Email={expectedEmailAddress}(?:\s|"">)";

        private sealed class EmailTable
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Subject { get; set; }
        }
    }
}
