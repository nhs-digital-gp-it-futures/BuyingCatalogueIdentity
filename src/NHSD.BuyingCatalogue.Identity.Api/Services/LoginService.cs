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

        private readonly IEventService eventService;
        private readonly IIdentityServerInteractionService interaction;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public LoginService(
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            this.eventService = eventService;
            this.interaction = interaction;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public void Dispose()
        {
            userManager?.Dispose();
        }

        public async Task<Result<SignInResponse>> SignInAsync(string username, string password, Uri returnUrl)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Result.Failure<SignInResponse>(LoginUserErrors.UserNameOrPasswordIncorrect());
            }

            var user = await userManager.FindByNameAsync(username);
            if (user is null)
            {
                return Result.Failure<SignInResponse>(LoginUserErrors.UserNameOrPasswordIncorrect());
            }

            var context = await interaction.GetAuthorizationContextAsync(returnUrl?.ToString());
            var isUserValidResult = await ValidateUserCredentialsAsync(user, password);
            if (!isUserValidResult.IsSuccess)
            {
                await eventService.RaiseAsync(new UserLoginFailureEvent(user.UserName, ValidateUserMessage, clientId: context?.ClientId));

                return Result.Failure<SignInResponse>(isUserValidResult.Errors);
            }

            var isUserDisabledResult = ValidateIfUserDisabled(user);

            if (!isUserDisabledResult.IsSuccess)
            {
                await eventService.RaiseAsync(new UserLoginFailureEvent(user.UserName, DisabledEventMessage, clientId: context?.ClientId));

                return Result.Failure<SignInResponse>(isUserDisabledResult.Errors);
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = context?.RedirectUri,
                AllowRefresh = true,
                IsPersistent = false,
            };

            await signInManager.SignInAsync(user, props);

            await RaiseLoginSuccessAsync(username, context);

            // We can trust returnUrl if GetAuthorizationContextAsync returned non-null
            return Result.Success(new SignInResponse(context is not null));
        }

        private static Result ValidateIfUserDisabled(ApplicationUser user)
        {
            return !user.Disabled ? Result.Success() : Result.Failure(LoginUserErrors.UserIsDisabled());
        }

        private async Task RaiseLoginSuccessAsync(string username, AuthorizationRequest context)
        {
            ApplicationUser user = await userManager.FindByNameAsync(username);
            await eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));
        }

        private async Task<Result> ValidateUserCredentialsAsync(ApplicationUser user, string password)
        {
            var result = await signInManager.CheckPasswordSignInAsync(user, password, true);
            return result.Succeeded ? Result.Success() : Result.Failure(LoginUserErrors.UserNameOrPasswordIncorrect());
        }
    }
}
