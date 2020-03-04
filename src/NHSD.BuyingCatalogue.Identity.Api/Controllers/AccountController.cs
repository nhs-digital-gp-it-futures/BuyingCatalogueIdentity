using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IClientStore clientStore,
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _clientStore = clientStore;
            _eventService = eventService;
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
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

            var returnUrl = viewModel.ReturnUrl?.ToString();
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            LoginViewModel NewLoginViewModel() =>
                new LoginViewModel { ReturnUrl = viewModel.ReturnUrl, Username = context?.LoginHint };

            if (!ModelState.IsValid)
                return View(NewLoginViewModel());

            var signedIn = await SignInAsync(viewModel, context);
            if (!signedIn)
                return View(NewLoginViewModel());

            await RaiseLoginSuccessAsync(viewModel, context);

            if (context == null)
                return LocalRedirect(returnUrl);

            if (await _clientStore.IsPkceClientAsync(context.ClientId))
            {
                // If the client is PKCE then we assume it's native, so this change in how to
                // return the response is for better UX for the end user.
                return View("Redirect", new RedirectViewModel(viewModel.ReturnUrl));
            }

            // We can trust viewModel.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(returnUrl);
        }

        public IActionResult Error()
        {
            return View("Error");
        }

        private async Task RaiseLoginSuccessAsync(LoginViewModel viewModel, AuthorizationRequest context)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(viewModel.Username);
            await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));
        }

        private async Task<bool> SignInAsync(LoginViewModel viewModel, AuthorizationRequest context)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, false, true);
            if (result.Succeeded)
                return true;

            await _eventService.RaiseAsync(new UserLoginFailureEvent(viewModel.Username, "invalid credentials", clientId: context?.ClientId));
            ModelState.AddModelError(string.Empty, "Invalid username or password");

            return false;
        }
    }
}
