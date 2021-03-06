﻿using System;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class LoginServiceBuilder
    {
        private IEventService eventService;
        private IIdentityServerInteractionService identityServerInteractionService;
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;

        internal LoginServiceBuilder()
        {
            eventService = Mock.Of<IEventService>();
            identityServerInteractionService = Mock.Of<IIdentityServerInteractionService>();
            userManager = CreateDefaultMockUserManager(ApplicationUserBuilder.Create().Build());
        }

        internal LoginService Build()
        {
            return new(
                eventService,
                identityServerInteractionService,
                signInManager,
                userManager);
        }

        internal LoginServiceBuilder WithAuthorizationContextResult(
            AuthorizationRequest getAuthorizationContextResult)
        {
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService
                .Setup(i => i.GetAuthorizationContextAsync(It.IsAny<string>()))
                .ReturnsAsync(getAuthorizationContextResult);

            identityServerInteractionService = mockInteractionService.Object;

            return this;
        }

        internal LoginServiceBuilder WithEventServiceCallback<T>(Action<T> eventCallback)
            where T : Event
        {
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(e => e.RaiseAsync(It.IsNotNull<T>())).Callback<Event>(e => eventCallback(e as T));

            eventService = mockEventService.Object;

            return this;
        }

        internal LoginServiceBuilder WithFindUserResult(ApplicationUser findByNameResult = null)
        {
            userManager = CreateDefaultMockUserManager(findByNameResult);
            return this;
        }

        internal LoginServiceBuilder WithSignInResult(IdentitySignInResult signInResult)
        {
            signInManager = CreateDefaultMockSignInManager(signInResult);
            return this;
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

        private SignInManager<ApplicationUser> CreateDefaultMockSignInManager(IdentitySignInResult signInResult)
        {
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null,
                null,
                null,
                null);

            mockSignInManager
                .Setup(s => s.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true))
                .ReturnsAsync(signInResult);

            return mockSignInManager.Object;
        }
    }
}
