using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly ScenarioContext _context;
        private readonly EmailServerDriver _emailServerDriver;
        private readonly Response _response;

        public EmailSteps(ScenarioContext context, Response response, EmailServerDriver emailServerDriver)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _response = response ?? throw new ArgumentNullException(nameof(response));
            _emailServerDriver = emailServerDriver ?? throw new ArgumentNullException(nameof(emailServerDriver));
        }

        [Then(@"the email sent contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedEmail = table.CreateInstance<EmailTable>();
            var actualEmail = (await _emailServerDriver.FindAllEmailsAsync()).First();

            actualEmail.PlainTextBody.Should().Contain(expectedEmail.ResetPasswordLink);
            actualEmail.HtmlBody.Should().Contain(expectedEmail.ResetPasswordLink);
            actualEmail.Should().BeEquivalentTo(expectedEmail, options => options.Excluding(e => e.ResetPasswordLink));
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

        private sealed class EmailTable
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Subject { get; set; }

            public string ResetPasswordLink { get; set; }
        }
    }
}
