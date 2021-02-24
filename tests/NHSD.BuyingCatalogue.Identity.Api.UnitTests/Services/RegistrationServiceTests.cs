using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.SharedMocks;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class RegistrationServiceTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IEmailService_IPasswordResetCallback_RegistrationSettings_NullEmailService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new RegistrationService(
                null,
                Mock.Of<IPasswordResetCallback>(),
                new RegistrationSettings()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IEmailService_IPasswordResetCallback_RegistrationSettings_NullPasswordResetCallback_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new RegistrationService(
                Mock.Of<IEmailService>(),
                null,
                new RegistrationSettings()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IEmailService_IPasswordResetCallback_RegistrationSettings_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new RegistrationService(
                Mock.Of<IEmailService>(),
                Mock.Of<IPasswordResetCallback>(),
                null));
        }

        [Test]
        public static void SendInitialEmailAsync_NullUser_ThrowsException()
        {
            static async Task SendEmail()
            {
                var emailService = new RegistrationService(
                    Mock.Of<IEmailService>(),
                    Mock.Of<IPasswordResetCallback>(),
                    new RegistrationSettings());

                await emailService.SendInitialEmailAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(SendEmail);
        }

        [Test]
        public static async Task SendInitialEmailAsync_SendsEmail()
        {
            var inputMessage = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                HtmlContent = "HTML",
                PlainTextContent = "Text",
            };

            var settings = new RegistrationSettings { EmailMessage = inputMessage };
            var mockEmailService = new Mock<IEmailService>();
            var mockPasswordResetCallback = Mock.Of<IPasswordResetCallback>(
                c => c.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()) == new Uri("https://www.google.co.uk/"));

            var user = ApplicationUserBuilder
                .Create()
                .WithEmailAddress("ricardo@burton.com")
                .Build();

            var registrationService = new RegistrationService(
                mockEmailService.Object,
                mockPasswordResetCallback,
                settings);

            await registrationService.SendInitialEmailAsync(new PasswordResetToken("Token", user));

            mockEmailService.Verify(e => e.SendEmailAsync(It.IsNotNull<EmailMessage>()));
        }

        [Test]
        public static async Task SendInitialEmailAsync_UsesExpectedTemplate()
        {
            // ReSharper disable once StringLiteralTypo
            const string subject = "Gozleme";

            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                Subject = subject,
            };

            var mockEmailService = new MockEmailService();
            var registrationService = new RegistrationService(
                mockEmailService,
                Mock.Of<IPasswordResetCallback>(),
                new RegistrationSettings { EmailMessage = template });

            await registrationService.SendInitialEmailAsync(
                new PasswordResetToken("Token", ApplicationUserBuilder.Create().Build()));

            mockEmailService.SentMessage.Subject.Should().Be(subject);
        }

        [Test]
        public static async Task SendInitialEmailAsync_UsesExpectedRecipient()
        {
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));
            var mockEmailService = new MockEmailService();

            var user = ApplicationUserBuilder
                .Create()
                .WithFirstName("Uncle")
                .WithLastName("Bob")
                .WithEmailAddress("uncle@bob.com")
                .Build();

            var registrationService = new RegistrationService(
                mockEmailService,
                Mock.Of<IPasswordResetCallback>(),
                new RegistrationSettings { EmailMessage = template });

            await registrationService.SendInitialEmailAsync(new PasswordResetToken("Token", user));

            var recipients = mockEmailService.SentMessage.Recipients;
            recipients.Should().HaveCount(1);

            var recipient = recipients[0];
            recipient.Address.Should().Be(user.Email);
            recipient.DisplayName.Should().Be(user.DisplayName);
        }

        [Test]
        public static async Task SendInitialEmailAsync_UsesExpectedCallback()
        {
            const string expectedCallback = "https://callback.nhs.uk/";

            var callback = new Uri(expectedCallback);
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

            var passwordResetCallback = Mock.Of<IPasswordResetCallback>(
                c => c.GetPasswordResetCallback(It.IsNotNull<PasswordResetToken>()) == callback);

            var mockEmailService = new MockEmailService();
            var registrationService = new RegistrationService(
                mockEmailService,
                passwordResetCallback,
                new RegistrationSettings { EmailMessage = template });

            await registrationService.SendInitialEmailAsync(
                new PasswordResetToken("Token", ApplicationUserBuilder.Create().Build()));

            mockEmailService.SentMessage.TextBody!.FormatItems.Should().HaveCount(1);
            mockEmailService.SentMessage.TextBody!.FormatItems[0].Should().Be(expectedCallback);
        }
    }
}
