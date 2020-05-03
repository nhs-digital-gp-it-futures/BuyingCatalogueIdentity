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
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<IPasswordResetCallback> _mockPasswordResetCallback;

        private ControllerContext _context;
        private ILoginService _loginService;
        private ILogoutService _logoutService;
        private DisabledErrorMessageSettings _disabledErrorMessageSettings;
        private IPublicBrowseSettings _publicBrowseSettings;

        private AccountControllerBuilder()
        {
            _context = Mock.Of<ControllerContext>();
            _loginService = Mock.Of<ILoginService>();
            _logoutService = Mock.Of<ILogoutService>();
            _mockPasswordResetCallback = new Mock<IPasswordResetCallback>();
            _mockPasswordService = new Mock<IPasswordService>();
            _disabledErrorMessageSettings = new DisabledErrorMessageSettings
            {
                EmailAddress = "Email",
                PhoneNumber = "Phone"
            };

            _publicBrowseSettings = Mock.Of<IPublicBrowseSettings>();
        }

        internal static AccountControllerBuilder Create()
        {
            return new AccountControllerBuilder();
        }

        internal AccountControllerBuilder WithDisabledErrorMessageSetting(DisabledErrorMessageSettings settings)
        {
            _disabledErrorMessageSettings = settings;
            return this;
        }

        internal AccountControllerBuilder WithLogoutService(ILogoutService logoutService)
        {
            _logoutService = logoutService;
            return this;
        }

        internal AccountControllerBuilder WithResetEmailCallback(Action<ApplicationUser, Uri> emailCallback)
        {
            _mockPasswordService.Setup(p => p.SendResetEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<Uri>()))
                .Callback(emailCallback);

            return this;
        }

        internal AccountControllerBuilder WithResetToken(PasswordResetToken token)
        {
            _mockPasswordService.Setup(p => p.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(token);

            return this;
        }

        internal AccountControllerBuilder WithScheme(string scheme)
        {
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Scheme).Returns(scheme);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            _context = new ControllerContext { HttpContext = mockHttpContext.Object };

            return this;
        }

        internal AccountControllerBuilder WithSignInResult(Result<SignInResponse> result)
        {
            var mockLoginService = new Mock<ILoginService>();
            mockLoginService.Setup(l => l.SignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .ReturnsAsync(result);

            _loginService = mockLoginService.Object;

            return this;
        }

        internal AccountControllerBuilder WithResetCallbackUrl(string url)
        {
            _mockPasswordResetCallback.Setup(c => c.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()))
                .Returns(new Uri(url));

            return this;
        }

        internal AccountControllerBuilder WithPasswordResetResult(IdentityResult result)
        {
            _mockPasswordService.Setup(x => x.ResetPasswordAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                ).ReturnsAsync(result);

            return this;
        }

        internal AccountControllerBuilder WithIsValidPasswordResetTokenResult()
        {
            _mockPasswordService.Setup(p => p.IsValidPasswordResetTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            return this;
        }

        internal AccountControllerBuilder WithPublicBrowseSettings(IPublicBrowseSettings publicBrowseSettings)
        {
            _publicBrowseSettings = publicBrowseSettings;
            return this;
        }

        internal AccountController Build()
        {
            return new AccountController(
                _loginService,
                _logoutService,
                _mockPasswordResetCallback.Object,
                _mockPasswordService.Object,
                _disabledErrorMessageSettings,
                _publicBrowseSettings)
            {
                ControllerContext = _context,
            };
        }
    }
}
