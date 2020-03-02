﻿using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogoutService _logoutService;

        public AccountController(
            IClientStore clientStore,
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogoutService logoutService)
        {
            _clientStore = clientStore;
            _eventService = eventService;
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
            _logoutService = logoutService;
        }

        [HttpGet]
        public IActionResult Login(Uri returnUrl)
        {
            if (returnUrl == null)
                returnUrl = new Uri("~/", UriKind.Relative);

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };

            ViewData["ReturnUrl"] = returnUrl;

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));

            var returnUrl = viewModel.ReturnUrl.ToString();
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            LoginViewModel NewLoginViewModel() =>
                new LoginViewModel { ReturnUrl = viewModel.ReturnUrl, Username = context?.LoginHint };

            if (!ModelState.IsValid)
                return View(NewLoginViewModel());

            var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, false, true);
            if (!result.Succeeded)
            {
                await _eventService.RaiseAsync(new UserLoginFailureEvent(viewModel.Username, "invalid credentials", clientId: context?.ClientId));
                ModelState.AddModelError(string.Empty, "Invalid username or password");

                return View(NewLoginViewModel());
            }

            var user = await _userManager.FindByNameAsync(viewModel.Username);
            await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));

            if (context != null)
            {
                if (await _clientStore.IsPkceClientAsync(context.ClientId))
                {
                    // If the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel(viewModel.ReturnUrl));
                }

                // We can trust viewModel.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl);
            }

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (string.IsNullOrEmpty(returnUrl))
                return Redirect("~/");

            // User might have clicked on a malicious link - should be logged
            throw new Exception("Invalid return URL");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            return await Logout(new LogoutViewModel
            {
                LogoutId = logoutId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel logoutViewModel)
        {
            logoutViewModel.ThrowIfNull(nameof(logoutViewModel));

            LogoutRequest logoutRequest = await _logoutService.GetLogoutRequest(logoutViewModel.LogoutId);

            await _logoutService.SignOutAsync(logoutRequest);

            return Redirect(logoutRequest?.PostLogoutRedirectUri);
        }

        [HttpGet]
        public async Task<IActionResult> Error(string errorId)
        {
            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);

            return Ok(message);
        }
    }
}
