using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Errors;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Common.Models.Results;
using ErrorMessage = NHSD.BuyingCatalogue.Identity.Common.Models.ErrorMessage;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    internal sealed class LoginService : ILoginService, IDisposable
    {
        internal const string ValidateUserMessage = "Invalid credentials";
        internal const string DisabledEventMessage = "User is disabled";

        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CookieExpirationSettings _cookieExpirationSettings;
        private List<ErrorMessage> errors = new List<ErrorMessage>();

        public LoginService(
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            CookieExpirationSettings cookieExpirationSettings)
        {
            _eventService = eventService;
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
            _cookieExpirationSettings = cookieExpirationSettings;
        }

        public void Dispose()
        {
            _userManager?.Dispose();
        }

        public async Task<Result<SignInResult>> SignInAsync(string username, string password, Uri returnUrl)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errors.Add(LoginUserErrors.UserNameOrPasswordIncorrect());
                return Result.Failure<SignInResult>(errors);
            }

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl?.ToString());

            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                errors.Add(LoginUserErrors.UserNameOrPasswordIncorrect());
                return Result.Failure<SignInResult>(errors);
            }

            var isValidResult = await ValidateUserAsync(user, password, context);
            if (!isValidResult)
            {
                return Result.Failure<SignInResult>(errors);
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = context?.RedirectUri,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_cookieExpirationSettings.ExpireTimeSpan.TotalMinutes),
                IsPersistent = false
            };

            await _signInManager.SignInAsync(user, props);

            await RaiseLoginSuccessAsync(username, context);

            // We can trust returnUrl if GetAuthorizationContextAsync returned non-null
            return Result.Success(context == null ? new SignInResult(true) : new SignInResult(true, true));
        }

        private async Task RaiseLoginSuccessAsync(string username, AuthorizationRequest context)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));
        }

        private async Task<bool> ValidateUserAsync(ApplicationUser user, string password, AuthorizationRequest context)
        {
          var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            if (result.Succeeded)
            {
                var isDisabled = await ValidateIfUserDisabledAsync(user, context);

                return isDisabled;
            }
            
            await _eventService.RaiseAsync(new UserLoginFailureEvent(user.UserName, ValidateUserMessage, clientId: context?.ClientId));
            errors.Add(LoginUserErrors.UserNameOrPasswordIncorrect());

            return false;
        }

        private async Task<bool> ValidateIfUserDisabledAsync(ApplicationUser user, AuthorizationRequest context)
        {
            if (!user.Disabled)
            {
                return true;
            }

            await _eventService.RaiseAsync(new UserLoginFailureEvent(user.UserName, DisabledEventMessage, clientId: context?.ClientId));

            errors.Add(LoginUserErrors.UserIsDisabled());

            return false;
        }
    }
}
