using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class EmailMessageTests
    {
        [Test]
        public void Constructor_EmailMessage_Uri_InitializesExpectedValues()
        {
            const string subject = "Subject";
            const string htmlBody = "HTML " + EmailMessage.ResetPasswordLinkPlaceholder;
            const string textBody = "Text " + EmailMessage.ResetPasswordLinkPlaceholder;
            const string url = "https://www.foobar.co.uk/";

            var sender = new EmailAddress();
            var inputMessage = new EmailMessage
            {
                Sender = sender,
                Subject = subject,
                HtmlBody = htmlBody,
                TextBody = textBody,
            };

            var outputMessage = new EmailMessage(inputMessage, new Uri(url));

            outputMessage.Sender.Should().Be(sender);
            outputMessage.Recipient.Should().BeNull();
            outputMessage.Subject.Should().Be(subject);
            outputMessage.HtmlBody.Should().Be("HTML " + url);
            outputMessage.TextBody.Should().Be("Text " + url);
        }

        [Test]
        public void Constructor_EmailMessage_Uri_UrlPlaceholderCaseMismatch_DoesNotSetUrl()
        {
            const string url = "https://www.foobar.co.uk/";

            var htmlBody = "HTML " + EmailMessage.ResetPasswordLinkPlaceholder.ToUpperInvariant();
            var textBody = "Text " + EmailMessage.ResetPasswordLinkPlaceholder.ToLowerInvariant();

            var inputMessage = new EmailMessage
            {
                Sender = new EmailAddress(),
                HtmlBody = htmlBody,
                TextBody = textBody,
            };

            var outputMessage = new EmailMessage(inputMessage, new Uri(url));

            outputMessage.HtmlBody.Should().Be(htmlBody);
            outputMessage.TextBody.Should().Be(textBody);
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Recipient_Set_NullAddress_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailMessage { Recipient = null });
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Sender_Set_NullAddress_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailMessage { Sender = null });
        }
    }
}
