﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MimeKit;
using NHSD.BuyingCatalogue.Identity.Common.Email;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Email
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class MimeKitExtensionsTests
    {
        [Test]
        public void AsMailboxAddress_ReturnsExpectedValue()
        {
            const string name = "Some Body";
            const string address = "somebody@notarealaddress.co.uk";

            var emailAddress = new EmailAddress(name, address);
            var mailboxAddress = emailAddress.AsMailboxAddress();

            mailboxAddress.Should().BeOfType<MailboxAddress>();
            mailboxAddress.Name.Should().Be(name);
            mailboxAddress.Address.Should().Be(address);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void AsMimeMessage_ReturnsExpectedValues_WithNoSubjectPrefix(string emailSubjectPrefix)
        {
            const string recipient = "recipient@somedomain.uk";
            const string sender = "sender@somedomain.nhs.uk";
            const string subject = "Subject";
            const string htmlBody = "HTML";
            const string textBody = "Text";

            var emailMessage = new EmailMessage
            {
                Sender = new EmailAddress { Address = sender },
                Recipient = new EmailAddress { Address = recipient },
                Subject = subject,
                HtmlBody = "HTML",
                TextBody = "Text",
            };

            var mimeMessage = emailMessage.AsMimeMessage(emailSubjectPrefix);

            mimeMessage.Should().BeOfType<MimeMessage>();

            IEnumerable<InternetAddress> from = mimeMessage.From;
            from.Should().HaveCount(1);
            from.First().As<MailboxAddress>().Address.Should().Be(sender);

            IEnumerable<InternetAddress> to = mimeMessage.To;
            to.Should().HaveCount(1);
            to.First().As<MailboxAddress>().Address.Should().Be(recipient);

            mimeMessage.Subject.Should().Be(subject);
            mimeMessage.HtmlBody.Should().Be(htmlBody);
            mimeMessage.TextBody.Should().Be(textBody);
        }

        [Test]
        public void AsMimeMessage_ReturnsExpectedValuesWithPrefix()
        {
            const string recipient = "recipient@somedomain.uk";
            const string sender = "sender@somedomain.nhs.uk";
            const string subject = "Subject";
            const string htmlBody = "HTML";
            const string textBody = "Text";
            const string emailSubjectPrefix = "Prefix";

            var emailMessage = new EmailMessage
            {
                Sender = new EmailAddress { Address = sender },
                Recipient = new EmailAddress { Address = recipient },
                Subject = subject,
                HtmlBody = "HTML",
                TextBody = "Text",
            };

            var mimeMessage = emailMessage.AsMimeMessage(emailSubjectPrefix);

            mimeMessage.Should().BeOfType<MimeMessage>();

            IEnumerable<InternetAddress> from = mimeMessage.From;
            from.Should().HaveCount(1);
            from.First().As<MailboxAddress>().Address.Should().Be(sender);

            IEnumerable<InternetAddress> to = mimeMessage.To;
            to.Should().HaveCount(1);
            to.First().As<MailboxAddress>().Address.Should().Be(recipient);

            mimeMessage.Subject.Should().Be($"{emailSubjectPrefix} {subject}");
            mimeMessage.HtmlBody.Should().Be(htmlBody);
            mimeMessage.TextBody.Should().Be(textBody);
        }

        [Test]
        public void AsMimeMessage_NullSubject_SetsSubjectToEmptyString()
        {
            var emailMessage = new EmailMessage
            {
                Sender = new EmailAddress { Address = "a@b.uk" },
                Recipient = new EmailAddress { Address = "a@b.uk" }
            };

            var mimeMessage = emailMessage.AsMimeMessage(string.Empty);

            mimeMessage.Should().BeOfType<MimeMessage>();
            mimeMessage.Subject.Should().Be(string.Empty);
        }
    }
}
