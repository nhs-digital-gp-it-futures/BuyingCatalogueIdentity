using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Errors;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AccountController : Controller
    {
        internal const string SignInErrorMessage = "Enter a valid email address and password";

        internal const string UserDisabledErrorMessageTemplate = @"There is a problem accessing your account.

Contact the account administrator at: {0} or call {1}";

        private readonly ILoginService _loginService;
        private readonly ILogoutService _logoutService;
        private readonly IPasswordResetCallback _passwordResetCallback;
        private readonly IPasswordService _passwordService;
        private readonly DisabledErrorMessageSettings _disabledErrorMessageSettings;

        public AccountController(
            ILoginService loginService,
            ILogoutService logoutService,
            IPasswordResetCallback passwordResetCallback,
            IPasswordService passwordService,
            DisabledErrorMessageSettings disabledErrorMessageSettings)
        {
            _loginService = loginService;
            _logoutService = logoutService;
            _passwordResetCallback = passwordResetCallback;
            _passwordService = passwordService;
            _disabledErrorMessageSettings = disabledErrorMessageSettings;
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
                var signInErrors = signInResult.Errors;

                if (signInErrors.Contains(LoginUserErrors.UserNameOrPasswordIncorrect()))
                {
                    ModelState.AddModelError(nameof(LoginViewModel.LoginError), SignInErrorMessage);
                }

                if (signInErrors.Contains(LoginUserErrors.UserIsDisabled()))
                {
                    var disabledErrorFormat = string.Format(
                        CultureInfo.CurrentCulture,
                        UserDisabledErrorMessageTemplate,
                        _disabledErrorMessageSettings.EmailAddress,
                        _disabledErrorMessageSettings.PhoneNumber);

                    ModelState.AddModelError(nameof(LoginViewModel.DisabledError), disabledErrorFormat);
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
        public IActionResult Registration(Uri returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword(Uri returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
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

            await _passwordService.SendResetEmailAsync(
                resetToken.User,
                _passwordResetCallback.GetPasswordResetCallback(resetToken));

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
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var res = await _passwordService.ResetPasswordAsync(viewModel.Email, viewModel.Token, viewModel.Password);
            if (res.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var invalidPasswordError = res.Errors.FirstOrDefault(error => error.Code == PasswordValidator.InvalidPasswordCode);
            if (invalidPasswordError != null)
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), invalidPasswordError.Description);
                return View(viewModel);
            }

            var invalidTokenError = res.Errors.FirstOrDefault(error => error.Code == PasswordService.InvalidTokenCode);
            if (invalidTokenError != null)
            {
                return RedirectToAction(nameof(ResetPasswordExpired));
            }

            throw new InvalidOperationException(
                $"Unexpected errors whilst resetting password: {string.Join(" & ", res.Errors.Select(error => error.Description))}");
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
