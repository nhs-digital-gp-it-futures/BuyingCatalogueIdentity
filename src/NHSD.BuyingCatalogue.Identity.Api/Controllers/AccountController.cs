using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AccountController : Controller
    {
        internal const string SignInErrorMessage = "Enter a valid email address and password";

        private readonly ILoginService _loginService;
        private readonly ILogoutService _logoutService;

        public AccountController(
            ILoginService loginService,
            ILogoutService logoutService)
        {
            _loginService = loginService;
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

            var signInResult = await _loginService.SignInAsync(viewModel.EmailAddress, viewModel.Password, viewModel.ReturnUrl);

            LoginViewModel NewLoginViewModel() =>
                new LoginViewModel { ReturnUrl = viewModel.ReturnUrl, EmailAddress = signInResult.LoginHint };

            if (!ModelState.IsValid)
                return View(NewLoginViewModel());

            if (!signInResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, SignInErrorMessage);
                return View(NewLoginViewModel());
            }

            var returnUrl = viewModel.ReturnUrl.ToString();

            if (signInResult.IsTrustedReturnUrl)

                // We can trust viewModel.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl);

            return LocalRedirect(returnUrl);
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

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("ForgotPasswordLinkSent");
        }

        [HttpGet]
        public IActionResult ForgotPasswordLinkSent()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));
            
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            return RedirectToAction("ResetPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
