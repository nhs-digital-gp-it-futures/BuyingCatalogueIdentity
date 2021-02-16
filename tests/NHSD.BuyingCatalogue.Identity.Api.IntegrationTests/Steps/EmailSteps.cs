using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Drivers;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly EmailServerDriver emailServerDriver;
        private readonly Settings settings;
        private readonly SeleniumContext seleniumContext;
        private readonly ScenarioContext context;

        public EmailSteps(EmailServerDriver emailServerDriver, Settings settings, SeleniumContext seleniumContext, ScenarioContext context)
        {
            this.emailServerDriver = emailServerDriver ?? throw new ArgumentNullException(nameof(emailServerDriver));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.seleniumContext = seleniumContext;
            this.context = context;
        }

        [Then(@"the email sent contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedEmail = table.CreateInstance<EmailTable>();
            var actualEmail = (await emailServerDriver.FindAllEmailsAsync()).Last();

            var urlRegex = PasswordResetUrlRegex(expectedEmail.To);
            actualEmail.PlainTextBody.Should().MatchRegex(urlRegex);
            actualEmail.HtmlBody.Should().MatchRegex(urlRegex);
            actualEmail.Should().BeEquivalentTo(expectedEmail);

            var linkParser = new Regex(urlRegex);
            var linkMatches = linkParser.Matches(actualEmail.PlainTextBody);
            linkMatches.Count.Should().Be(1, "there should only be one link in the email");

            var link = linkMatches.First().Value;
            context["EmailLink"] = link;
        }

        [Then(@"no email is sent")]
        public async Task EmailIsNotSent()
        {
            var emails = await emailServerDriver.FindAllEmailsAsync();
            emails.Should().BeNullOrEmpty();
        }

        [Then(@"only one email is sent")]
        public async Task OnlyOneEmailIsSent()
        {
            context[ScenarioContextKeys.EmailSent] = true;
            var actualCount = await emailServerDriver.GetEmailCountAsync();
            actualCount.Should().Be(1);
        }

        [When(@"The email link is modified to use email address (.*)")]
        public void WhenTheEmailLinkIsModified(string email)
        {
            var link = context["EmailLink"].As<string>();
            var index = link.IndexOf("Email=", StringComparison.InvariantCulture);
            var trimmedString = link.Substring(0, index);

            context["EmailLink"] = $"{trimmedString}Email={email}";
        }

        [When(@"The email link is clicked")]
        public void WhenTheEmailLinkIsClicked()
        {
            var link = context["EmailLink"].As<string>();
            seleniumContext.WebDriver.Navigate().GoToUrl(new Uri(link));
        }

        private string PasswordResetUrlRegex(string expectedEmailAddress) =>
            $@"(?:\s|<a href=""){settings.IdentityApiBaseUrl}Account/ResetPassword\?Token=(?:\S+)&Email={expectedEmailAddress}(?:\s|"">)";

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class EmailTable
        {
            public string From { get; init; }

            public string To { get; init; }

            public string Subject { get; init; }
        }
    }
}
