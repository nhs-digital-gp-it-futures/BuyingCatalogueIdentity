using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.SharedMocks;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class EmailServiceExtensionsTests
    {
        [Test]
        public static async Task SendEmailAsync_MessageHasExpectedRecipient()
        {
            var recipient = new EmailAddress();
            var service = new MockEmailService();
            var template = new EmailMessageTemplate { Sender = new EmailAddress() };

            await service.SendEmailAsync(template, recipient);

            service.SentMessage.Recipients.Should().HaveCount(1);
            service.SentMessage.Recipients[0].Should().BeSameAs(recipient);
        }

        [Test]
        public static async Task SendEmailAsync_MessageHasExpectedFormatItems()
        {
            object[] formatItems = { 1, "2" };
            var service = new MockEmailService();
            var template = new EmailMessageTemplate
            {
                Sender = new EmailAddress(),
                HtmlBody = new EmailMessageBody(),
                TextBody = new EmailMessageBody(),
            };

            await service.SendEmailAsync(template, new EmailAddress(), formatItems);

            service.SentMessage.HtmlBody!.FormatItems.Should().BeEquivalentTo(formatItems);
            service.SentMessage.TextBody!.FormatItems.Should().BeEquivalentTo(formatItems);
        }

        [Test]
        public static async Task SendEmailAsync_UsesMessageTemplate()
        {
            const string subject = "Banitsa";

            var service = new MockEmailService();
            var template = new EmailMessageTemplate
            {
                Sender = new EmailAddress(),
                Subject = subject,
            };

            await service.SendEmailAsync(template, new EmailAddress());

            service.SentMessage.Subject.Should().Be(subject);
        }
    }
}
