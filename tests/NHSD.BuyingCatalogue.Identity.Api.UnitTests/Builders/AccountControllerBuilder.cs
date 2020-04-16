using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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

        private ControllerContext _context;
        private ILoginService _loginService;
        private ILogoutService _logoutService;
        private IUrlHelper _urlHelper;
        private DisabledErrorMessageSettings _disabledErrorMessageSettings;

        internal AccountControllerBuilder()
        {
            _context = Mock.Of<ControllerContext>();
            _loginService = Mock.Of<ILoginService>();
            _logoutService = Mock.Of<ILogoutService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _urlHelper = Mock.Of<IUrlHelper>();
            _disabledErrorMessageSettings = new DisabledErrorMessageSettings
            {
                EmailAddress = "Email",
                PhoneNumber = "Phone"
            };
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

        internal AccountControllerBuilder WithResetEmailCallback(Action<ApplicationUser, string> emailCallback)
        {
            _mockPasswordService.Setup(p => p.SendResetEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
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

        internal AccountControllerBuilder WithUrlAction(string action)
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(h => h.Action(It.IsNotNull<UrlActionContext>()))
                .Returns(action);

            _urlHelper = mockUrlHelper.Object;

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

        internal AccountControllerBuilder WithUrlActionCallback(Action<UrlActionContext> actionCallback)
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(h => h.Action(It.IsNotNull<UrlActionContext>()))
                .Callback(actionCallback);

            _urlHelper = mockUrlHelper.Object;

            return this;
        }

        internal AccountController Build()
        {
            return new AccountController(_loginService, _logoutService, _mockPasswordService.Object, _disabledErrorMessageSettings)
            {
                ControllerContext = _context,
                Url = _urlHelper,
            };
        }
    }
}
