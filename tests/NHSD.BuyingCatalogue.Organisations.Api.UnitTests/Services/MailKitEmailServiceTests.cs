using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MailKit;
using MailKit.Security;
using MimeKit;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class MailKitEmailServiceTests
    {
        [Test]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public void Constructor_IMailTransport_SmtpSettings_AllowInvalidCertificateFalse_DoesNotSetCertificateValidationCallback()
        {
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.SetupProperty(t => t.ServerCertificateValidationCallback);

            new MailKitEmailService(mockTransport.Object, new SmtpSettings());

            mockTransport.Object.ServerCertificateValidationCallback.Should().BeNull();
        }

        [Test]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public void Constructor_IMailTransport_SmtpSettings_AllowInvalidCertificateTrue_SetsCertificateValidationCallback()
        {
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.SetupProperty(t => t.ServerCertificateValidationCallback);

            new MailKitEmailService(mockTransport.Object, new SmtpSettings { AllowInvalidCertificate = true });

            var callback = mockTransport.Object.ServerCertificateValidationCallback;

            callback.Invoke(null, null, null, SslPolicyErrors.None).Should().BeTrue();
        }

        [Test]
        public async Task SendEmailAsync_ConnectsWithExpectedSettings()
        {
            const string host = "host";
            const int port = 125;

            var settings = new SmtpSettings
            {
                Authentication = new SmtpAuthenticationSettings(),
                Host = host,
                Port = port,
            };

            var mockTransport = new Mock<IMailTransport>();

            var service = new MailKitEmailService(mockTransport.Object, settings);

            await service.SendEmailAsync(
                new EmailMessage
                {
                    Sender = new EmailAddress { Address = "from@sender.uk" },
                    Recipient = new EmailAddress { Address = "to@recipient.uk" },
                    Subject = "subject",
                });

            mockTransport.Verify(
                t => t.ConnectAsync(
                    It.Is<string>(h => h == settings.Host),
                    It.Is<int>(p => p == settings.Port),
                    It.Is<SecureSocketOptions>(s => s == SecureSocketOptions.Auto),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public async Task SendEmailAsync_RequireAuthenticatioIsTrue_AuthenticatesUsingExpectedCredentials()
        {
            const string userName = "User";
            const string password = "Password";

            var authentication = new SmtpAuthenticationSettings
            {
                IsRequired = true,
                UserName = userName,
                Password = password,
            };

            var settings = new SmtpSettings { Authentication = authentication };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(mockTransport.Object, settings);

            await service.SendEmailAsync(
                new EmailMessage
                {
                    Sender = new EmailAddress { Address = "from@sender.uk" },
                    Recipient = new EmailAddress { Address = "to@recipient.uk" },
                    Subject = "subject",
                });

            mockTransport.Verify(
                t => t.AuthenticateAsync(
                    It.Is<string>(u => u == authentication.UserName),
                    It.Is<string>(p => p == authentication.Password),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public async Task SendEmailAsync_RequireAuthenticatioIsFalse_DoesNotAuthenticate()
        {
            var settings = new SmtpSettings { Authentication = new SmtpAuthenticationSettings() };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(mockTransport.Object, settings);

            await service.SendEmailAsync(
                new EmailMessage
                {
                    Sender = new EmailAddress { Address = "from@sender.uk" },
                    Recipient = new EmailAddress { Address = "to@recipient.uk" },
                    Subject = "subject",
                });

            mockTransport.Verify(
                t => t.AuthenticateAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Test]
        public async Task SendEmailAsync_SendsExpectedMessage()
        {
            var settings = new SmtpSettings { Authentication = new SmtpAuthenticationSettings() };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(mockTransport.Object, settings);

            var subject = Guid.NewGuid().ToString();

            await service.SendEmailAsync(
                new EmailMessage
                {
                    Sender = new EmailAddress { Address = "from@sender.uk" },
                    Recipient = new EmailAddress { Address = "to@recipient.uk" },
                    Subject = subject,
                });

            mockTransport.Verify(
                t => t.SendAsync(
                    It.Is<MimeMessage>(m => m.Subject == subject),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()),
                Times.Once());
        }

        [Test]
        public async Task SendEmailAsync_Disconnects()
        {
            var settings = new SmtpSettings { Authentication = new SmtpAuthenticationSettings() };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(mockTransport.Object, settings);

            await service.SendEmailAsync(
                new EmailMessage
                {
                    Sender = new EmailAddress { Address = "from@sender.uk" },
                    Recipient = new EmailAddress { Address = "to@recipient.uk" },
                    Subject = "subject",
                });

            mockTransport.Verify(
                t => t.DisconnectAsync(
                    It.Is<bool>(q => q),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
