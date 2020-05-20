using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Errors;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;

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

        public async Task<Result<SignInResponse>> SignInAsync(string username, string password, Uri returnUrl)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Result.Failure<SignInResponse>(LoginUserErrors.UserNameOrPasswordIncorrect());
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return Result.Failure<SignInResponse>(LoginUserErrors.UserNameOrPasswordIncorrect());
            }

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl?.ToString());
            var isUserValidResult = await ValidateUserCredentialsAsync(user, password);
            if (!isUserValidResult.IsSuccess)
            {
                await _eventService.RaiseAsync(new UserLoginFailureEvent(user.UserName, ValidateUserMessage, clientId: context?.ClientId));

                return Result.Failure<SignInResponse>(isUserValidResult.Errors);
            }

            var isUserDisabledResult = ValidateIfUserDisabled(user);

            if (!isUserDisabledResult.IsSuccess)
            {
                await _eventService.RaiseAsync(new UserLoginFailureEvent(user.UserName, DisabledEventMessage, clientId: context?.ClientId));

                return Result.Failure<SignInResponse>(isUserDisabledResult.Errors);
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = context?.RedirectUri,
                AllowRefresh = true,
                IsPersistent = false
            };

            await _signInManager.SignInAsync(user, props);

            await RaiseLoginSuccessAsync(username, context);

            // We can trust returnUrl if GetAuthorizationContextAsync returned non-null
            return Result.Success(new SignInResponse(context is object));
        }

        private async Task RaiseLoginSuccessAsync(string username, AuthorizationRequest context)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));
        }

        private async Task<Result> ValidateUserCredentialsAsync(ApplicationUser user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            return result.Succeeded ? Result.Success() : Result.Failure(LoginUserErrors.UserNameOrPasswordIncorrect());
        }

        private static Result ValidateIfUserDisabled(ApplicationUser user)
        {
            return !user.Disabled ? Result.Success() : Result.Failure(LoginUserErrors.UserIsDisabled());
        }
    }
}
