using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Common.Email;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    internal sealed class PasswordServiceTests
    {
        private static Mock<IUserStore<ApplicationUser>> MockUserStore => new Mock<IUserStore<ApplicationUser>>();
        private static Mock<UserManager<ApplicationUser>> MockUserManager => new Mock<UserManager<ApplicationUser>>(
        MockUserStore.Object,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null);

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Constructor exception testing")]
        public void Constructor_IEmailService_PasswordResetSettings_UserManagerApplicationUser_NullEmailService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new PasswordService(
                    null,
                    new PasswordResetSettings(),
                    MockUserManager.Object));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Constructor exception testing")]
        public void Constructor_IEmailService_PasswordResetSettings_UserManagerApplicationUser_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new PasswordService(
                    Mock.Of<IEmailService>(),
                    null,
                    MockUserManager.Object));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Constructor exception testing")]
        public void Constructor_IEmailService_PasswordResetSettings_UserManagerApplicationUser_NullUserManager_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    null));
        }

        [Test]
        public void GeneratePasswordResetTokenAsync_NullEmailAddress_ThrowsException()
        {
            static async Task GeneratePasswordResetTokenAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.GeneratePasswordResetTokenAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(GeneratePasswordResetTokenAsync);
        }

        [TestCase("")]
        [TestCase("\t")]
        public void GeneratePasswordResetTokenAsync_EmptyOrWhiteSpaceEmailAddress_ThrowsException(string emailAddress)
        {
            async Task GeneratePasswordResetTokenAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.GeneratePasswordResetTokenAsync(emailAddress);
            }

            Assert.ThrowsAsync<ArgumentException>(GeneratePasswordResetTokenAsync);
        }

        [Test]
        public async Task GeneratePasswordResetTokenAsync_UserNotFound_ReturnsNull()
        {
            var service = new PasswordService(
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                MockUserManager.Object);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().BeNull();
        }

        [Test]
        public async Task GeneratePasswordResetTokenAsync_UserFound_ReturnsExpectedToken()
        {
            const string emailAddress = "a@b.com";
            const string expectedToken = "HereBeToken";
            var expectedUser = new ApplicationUser();

            var mockUserManager = MockUserManager;
            mockUserManager.Setup(m => m.FindByEmailAsync(It.Is<string>(e => e == emailAddress)))
                .ReturnsAsync(expectedUser);

            mockUserManager
                .Setup(m => m.GeneratePasswordResetTokenAsync(It.Is<ApplicationUser>(u => u == expectedUser)))
                .ReturnsAsync(expectedToken);

            var service = new PasswordService(
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                mockUserManager.Object);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().NotBeNull();
            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }

        [Test]
        public void SendResetEmailAsync_NullUser_ThrowsException()
        {
            static async Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.SendResetEmailAsync(null, "callback");
            }

            Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [Test]
        public void SendResetEmailAsync_NullCallback_ThrowsException()
        {
            static async Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.SendResetEmailAsync(new ApplicationUser(), null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [TestCase("")]
        [TestCase("\t")]
        public void SendResetEmailAsync_EmptyOrWhiteSpaceCallback_ThrowsException(string callback)
        {
            async Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.SendResetEmailAsync(new ApplicationUser(), callback);
            }

            Assert.ThrowsAsync<ArgumentException>(SendResetEmailAsync);
        }

        [Test]
        public async Task SendResetEmailAsync_SendsExpectedEmail()
        {
            const string emailAddress = "a@b.com";
            const string expectedCallback = "https://identity/account/resetPassword?token=1234&emailAddress=a@b.com";
            var expectedUser = new ApplicationUser
            {
                Email = emailAddress,
                FirstName = "Eggs",
                LastName = "Benedict",
            };

            var expectedSender = new EmailAddress("Uncle Robert", "uncle@bob.com");
            const string expectedSubject = "Password Reset";

            var configuredMessage = new EmailMessage
            {
                Sender = expectedSender,
                Subject = expectedSubject,
                HtmlBody = EmailMessage.ResetPasswordLinkPlaceholder,
                TextBody = EmailMessage.ResetPasswordLinkPlaceholder,
            };

            var emailCount = 0;
            EmailMessage actualEmailMessage = null;

            void EmailCallback(EmailMessage message)
            {
                emailCount++;
                actualEmailMessage = message;
            }

            var mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(e => e.SendEmailAsync(It.IsNotNull<EmailMessage>()))
                .Callback<EmailMessage>(EmailCallback);

            var settings = new PasswordResetSettings { EmailMessage = configuredMessage };
            var service = new PasswordService(
                mockEmailService.Object,
                settings,
                MockUserManager.Object);

            await service.SendResetEmailAsync(expectedUser, expectedCallback);

            emailCount.Should().Be(1);

            actualEmailMessage.Should().NotBeNull();
            actualEmailMessage.Sender.Should().Be(expectedSender);
            actualEmailMessage.Recipient.Should().BeEquivalentTo(
                new EmailAddress(expectedUser.DisplayName, expectedUser.Email));

            actualEmailMessage.Subject.Should().Be(expectedSubject);
            actualEmailMessage.HtmlBody.Should().Be(expectedCallback);
            actualEmailMessage.TextBody.Should().Be(expectedCallback);
        }

        [Test]
        public async Task ResetPasswordAsync_WithUser_ReturnsIdentityResult()
        {
            var email = "a@b.c";
            var token = "I am a token, honest!";
            var password = "Pass123321";
            var expectedResult = new IdentityResult();
            var user = new ApplicationUser();
            var mockUserManager = MockUserManager;
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            mockUserManager.Setup(x => x.ResetPasswordAsync(user, token, password)).ReturnsAsync(() => expectedResult);

            var service = new PasswordService(
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                mockUserManager.Object);

            var result = await service.ResetPasswordAsync(email, token, password);
            result.Should().Be(expectedResult);
        }
    }
}
