using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogoutService _logoutService;

        public AccountController(
            IEventService eventService,
            IIdentityServerInteractionService interaction,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogoutService logoutService)
        {
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

            var returnUrl = viewModel.ReturnUrl?.ToString();
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

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

            // We can trust viewModel.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(returnUrl);
        }
            
		[HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (string.IsNullOrWhiteSpace(logoutId))
            {
                throw new ArgumentNullException(nameof(logoutId));
            }

            LogoutRequest logoutRequest = await _logoutService.GetLogoutRequestAsync(logoutId);

            await _logoutService.SignOutAsync(logoutRequest);

            return Redirect(logoutRequest?.PostLogoutRedirectUri);
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
