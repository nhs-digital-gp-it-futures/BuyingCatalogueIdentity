using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using SignInResult = NHSD.BuyingCatalogue.Identity.Api.Services.SignInResult;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class AccountControllerBuilder
    {
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<IPasswordResetCallback> _mockPasswordResetCallback;

        private ControllerContext _context;
        private ILoginService _loginService;
        private ILogoutService _logoutService;

        internal AccountControllerBuilder()
        {
            _context = Mock.Of<ControllerContext>();
            _loginService = Mock.Of<ILoginService>();
            _logoutService = Mock.Of<ILogoutService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _mockPasswordResetCallback = new Mock<IPasswordResetCallback>();
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

        internal AccountControllerBuilder WithSignInResult(SignInResult result)
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

        internal AccountController Build()
        {
            return new AccountController(
                _loginService,
                _logoutService,
                _mockPasswordResetCallback.Object,
                _mockPasswordService.Object)
            {
                ControllerContext = _context,
            };
        }
    }
}
