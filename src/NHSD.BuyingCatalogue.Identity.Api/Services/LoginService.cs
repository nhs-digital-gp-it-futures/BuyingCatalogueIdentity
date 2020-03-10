using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    internal sealed class LoginService : ILoginService, IDisposable
    {
        internal const string EventMessage = "Invalid credentials";

        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginService(
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void Dispose()
        {
            _userManager?.Dispose();
        }

        public async Task<SignInResult> SignInAsync(string username, string password, Uri returnUrl)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return new SignInResult(false);

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl?.ToString());

            var signedIn = await SignInAsync(username, password, context);
            if (!signedIn)
                return new SignInResult(false, loginHint: context?.LoginHint);

            await RaiseLoginSuccessAsync(username, context);

            // We can trust returnUrl if GetAuthorizationContextAsync returned non-null
            return context == null ? new SignInResult(true) : new SignInResult(true, true);
        }

        private async Task RaiseLoginSuccessAsync(string username, AuthorizationRequest context)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));
        }

        private async Task<bool> SignInAsync(string username, string password, AuthorizationRequest context)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, true);
            if (result.Succeeded)
                return true;

            await _eventService.RaiseAsync(new UserLoginFailureEvent(username, EventMessage, clientId: context?.ClientId));

            return false;
        }
    }
}
