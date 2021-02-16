using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class AccountControllerBuilder
    {
        private readonly Mock<IPasswordService> mockPasswordService;
        private readonly Mock<IPasswordResetCallback> mockPasswordResetCallback;

        private ControllerContext context;
        private ILoginService loginService;
        private ILogoutService logoutService;
        private DisabledErrorMessageSettings disabledErrorMessageSettings;
        private PublicBrowseSettings publicBrowseSettings;

        private AccountControllerBuilder()
        {
            context = Mock.Of<ControllerContext>();
            loginService = Mock.Of<ILoginService>();
            logoutService = Mock.Of<ILogoutService>();
            mockPasswordResetCallback = new Mock<IPasswordResetCallback>();
            mockPasswordService = new Mock<IPasswordService>();
            disabledErrorMessageSettings = new DisabledErrorMessageSettings
            {
                EmailAddress = "Email",
                PhoneNumber = "Phone",
            };

            publicBrowseSettings = new PublicBrowseSettings();
        }

        internal static AccountControllerBuilder Create()
        {
            return new();
        }

        internal AccountControllerBuilder WithDisabledErrorMessageSetting(DisabledErrorMessageSettings settings)
        {
            disabledErrorMessageSettings = settings;
            return this;
        }

        internal AccountControllerBuilder WithLogoutService(ILogoutService service)
        {
            logoutService = service;
            return this;
        }

        internal AccountControllerBuilder WithResetEmailCallback(Action<ApplicationUser, Uri> emailCallback)
        {
            mockPasswordService.Setup(p => p.SendResetEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<Uri>()))
                .Callback(emailCallback);

            return this;
        }

        internal AccountControllerBuilder WithResetToken(PasswordResetToken token)
        {
            mockPasswordService.Setup(p => p.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(token);

            return this;
        }

        internal AccountControllerBuilder WithScheme(string scheme)
        {
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Scheme).Returns(scheme);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            context = new ControllerContext { HttpContext = mockHttpContext.Object };

            return this;
        }

        internal AccountControllerBuilder WithSignInResult(Result<SignInResponse> result)
        {
            var mockLoginService = new Mock<ILoginService>();
            mockLoginService
                .Setup(l => l.SignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .ReturnsAsync(result);

            loginService = mockLoginService.Object;

            return this;
        }

        internal AccountControllerBuilder WithResetCallbackUrl(string url)
        {
            mockPasswordResetCallback
                .Setup(c => c.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()))
                .Returns(new Uri(url));

            return this;
        }

        internal AccountControllerBuilder WithPasswordResetResult(IdentityResult result)
        {
            mockPasswordService
                .Setup(s => s.ResetPasswordAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(result);

            return this;
        }

        internal AccountControllerBuilder WithIsValidPasswordResetTokenResult()
        {
            mockPasswordService
                .Setup(p => p.IsValidPasswordResetTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            return this;
        }

        internal AccountControllerBuilder WithPublicBrowseSettings(PublicBrowseSettings settings)
        {
            publicBrowseSettings = settings;
            return this;
        }

        internal AccountController Build()
        {
            return new(
                loginService,
                logoutService,
                mockPasswordResetCallback.Object,
                mockPasswordService.Object,
                disabledErrorMessageSettings,
                publicBrowseSettings)
            {
                ControllerContext = context,
            };
        }
    }
}
