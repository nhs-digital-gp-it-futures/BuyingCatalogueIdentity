using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NHSD.BuyingCatalogue.Identity.Api.Errors;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AccountController : Controller
    {
        internal const string SignInErrorMessage = "Enter a valid email address and password";

        internal const string UserDisabledErrorMessage = @"There is a problem accessing your account.

Contact the account administrator at: exeter.helpdesk@nhs.net or call 0300 303 4034";

        private readonly ILoginService _loginService;
        private readonly ILogoutService _logoutService;
        private readonly IPasswordService _passwordService;

        public AccountController(
            ILoginService loginService,
            ILogoutService logoutService,
            IPasswordService passwordService)
        {
            _loginService = loginService;
            _logoutService = logoutService;
            _passwordService = passwordService;
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
                new LoginViewModel { ReturnUrl = viewModel.ReturnUrl, EmailAddress = signInResult.Value?.LoginHint };

            if (!ModelState.IsValid)
                return View(NewLoginViewModel());

            if (!signInResult.IsSuccess)
            {
                if (signInResult.Errors.Contains(LoginUserErrors.UserNameOrPasswordIncorrect()))
                {
                    ModelState.AddModelError(nameof(LoginViewModel.LoginError), SignInErrorMessage);
                }
                if (signInResult.Errors.Contains(LoginUserErrors.UserIsDisabled()))
                {
                    ModelState.AddModelError(nameof(LoginViewModel.DisabledError), UserDisabledErrorMessage);
                }

                return View(NewLoginViewModel());
            }

            var returnUrl = viewModel.ReturnUrl.ToString();

            // We can trust viewModel.ReturnUrl since GetAuthorizationContextAsync returned non-null
            if (signInResult.Value.IsTrustedReturnUrl)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var resetToken = await _passwordService.GeneratePasswordResetTokenAsync(viewModel.EmailAddress);
            if (resetToken == null)
                return RedirectToAction(nameof(ForgotPasswordLinkSent));

            var callback = Url.Action(
                new UrlActionContext
                {
                    Action = nameof(ResetPassword),
                    Protocol = Request.Scheme,
                    Values = new { resetToken.Token, Email = viewModel.EmailAddress }
                });

            await _passwordService.SendResetEmailAsync(resetToken.User, callback);

            return RedirectToAction(nameof(ForgotPasswordLinkSent));
        }

        [HttpGet]
        public IActionResult ForgotPasswordLinkSent()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordExpired()
        {
            return View();
        }
    }
}
