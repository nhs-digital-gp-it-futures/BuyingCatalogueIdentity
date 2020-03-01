using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class AccountControllerBuilder
    {
        private IClientStore _clientStore;
        private IEventService _eventService;
        private IIdentityServerInteractionService _identityServerInteractionService;
        private ILogoutService _logoutService;

        internal AccountControllerBuilder()
        {
            _clientStore = Mock.Of<IClientStore>();
            _eventService = Mock.Of<IEventService>();
            _identityServerInteractionService = Mock.Of<IIdentityServerInteractionService>();
            _logoutService = Mock.Of<ILogoutService>();
        }

        internal AccountControllerBuilder WithClientStore(IClientStore clientStore)
        {
            _clientStore = clientStore;
            return this;
        }

        internal AccountControllerBuilder WithEventService(IEventService eventService)
        {
            _eventService = eventService;
            return this;
        }

        internal AccountControllerBuilder WithIdentityServerInteractionService(IIdentityServerInteractionService identityServerInteractionService)
        {
            _identityServerInteractionService = identityServerInteractionService;
            return this;
        }

        internal AccountControllerBuilder WithLogoutService(ILogoutService logoutService)
        {
            _logoutService = logoutService;
            return this;
        }

        internal AccountController Build()
        {
            return new AccountController(_clientStore, _eventService, _identityServerInteractionService, null, null, _logoutService);
        }
    }
}
