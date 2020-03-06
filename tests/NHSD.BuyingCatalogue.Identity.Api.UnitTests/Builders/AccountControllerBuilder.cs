using System;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using SignInResult = NHSD.BuyingCatalogue.Identity.Api.Services.SignInResult;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class AccountControllerBuilder
    {
        private ILoginService _loginService;
        private ILogoutService _logoutService;

        internal AccountControllerBuilder()
        {
            _loginService = Mock.Of<ILoginService>();
            _logoutService = Mock.Of<ILogoutService>();
        }

        internal AccountControllerBuilder WithLogoutService(ILogoutService logoutService)
        {
            _logoutService = logoutService;
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

        internal AccountController Build()
        {
            return new AccountController(_loginService, _logoutService);
        }
    }
}
