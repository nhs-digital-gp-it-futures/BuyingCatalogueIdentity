using System;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class AccountControllerBuilder
    {
        private IEventService _eventService;
        private IIdentityServerInteractionService _identityServerInteractionService;
        private ILogoutService _logoutService;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;

        internal AccountControllerBuilder()
        {
            _eventService = Mock.Of<IEventService>();
            _identityServerInteractionService = Mock.Of<IIdentityServerInteractionService>();
            _logoutService = Mock.Of<ILogoutService>();
            _userManager = CreateDefaultMockUserManager(new ApplicationUser());
        }

        internal AccountControllerBuilder WithEventService(IEventService eventService)
        {
            _eventService = eventService;
            return this;
        }

        internal AccountControllerBuilder WithEventService<T>(Action<T> eventCallback)
            where T : Event
        {
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(e => e.RaiseAsync(It.IsNotNull<T>()))
                .Callback<Event>(e => eventCallback(e as T));

            _eventService = mockEventService.Object;

            return this;
        }

        internal AccountControllerBuilder WithIdentityServerInteractionService(IIdentityServerInteractionService identityServerInteractionService)
        {
            _identityServerInteractionService = identityServerInteractionService;
            return this;
        }

        internal AccountControllerBuilder WithIdentityServerInteractionService(AuthorizationRequest getAuthorizationContextResult)
        {
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService
                .Setup(i => i.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync(getAuthorizationContextResult);

            _identityServerInteractionService = mockInteractionService.Object;

            return this;
        }

        internal AccountControllerBuilder WithIdentityServerInteractionService(
            AuthorizationRequest getAuthorizationContextResult,
            string expectedReturnUrl)
        {
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService
                .Setup(i => i.GetAuthorizationContextAsync(It.Is<string>(r => r == expectedReturnUrl)))
                .ReturnsAsync(getAuthorizationContextResult);

            _identityServerInteractionService = mockInteractionService.Object;

            return this;
        }

        internal AccountControllerBuilder WithLogoutService(ILogoutService logoutService)
        {
            _logoutService = logoutService;
            return this;
        }

        internal AccountControllerBuilder WithSignInManager(SignInResult signInResult)
        {
            _signInManager = CreateDefaultMockSignInManager(signInResult);
            return this;
        }

        internal AccountControllerBuilder WithUserManager(ApplicationUser findByNameResult = null)
        {
            _userManager = CreateDefaultMockUserManager(findByNameResult);
            return this;
        }

        internal AccountController Build()
        {
            return new AccountController(
                _eventService,
                _identityServerInteractionService,
                _signInManager,
                _userManager,
                _logoutService);
        }

        private static UserManager<ApplicationUser> CreateDefaultMockUserManager(ApplicationUser findByNameResult)
        {
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            mockUserManager
                .Setup(s => s.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(findByNameResult);

            return mockUserManager.Object;
        }

        private SignInManager<ApplicationUser> CreateDefaultMockSignInManager(SignInResult signInResult)
        {
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _userManager,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null,
                null,
                null,
                null);

            mockSignInManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(signInResult);

            return mockSignInManager.Object;
        }
    }
}
