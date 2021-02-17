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

        private readonly ILoginService loginService;
        private readonly ILogoutService logoutService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly IPasswordService passwordService;
        private readonly DisabledErrorMessageSettings disabledErrorMessageSettings;
        private readonly PublicBrowseSettings publicBrowseSettings;

        public AccountController(
            ILoginService loginService,
            ILogoutService logoutService,
            IPasswordResetCallback passwordResetCallback,
            IPasswordService passwordService,
            DisabledErrorMessageSettings disabledErrorMessageSettings,
            PublicBrowseSettings publicBrowseSettings)
        {
            this.loginService = loginService;
            this.logoutService = logoutService;
            this.passwordResetCallback = passwordResetCallback;
            this.passwordService = passwordService;
            this.disabledErrorMessageSettings = disabledErrorMessageSettings;
            this.publicBrowseSettings = publicBrowseSettings;
        }

        [HttpGet]
        public IActionResult Login(Uri returnUrl)
        {
            if (returnUrl is null)
                return RedirectToPublicBrowseLogin();

            var loginViewModel = new LoginViewModel { ReturnUrl = returnUrl };

            ViewData["ReturnUrl"] = returnUrl;

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var returnUri = viewModel.ReturnUrl;

            if (returnUri is null)
                return RedirectToPublicBrowseLogin();

            var signInResult = await loginService.SignInAsync(viewModel.EmailAddress, viewModel.Password, returnUri);

            LoginViewModel NewLoginViewModel() =>
                new() { ReturnUrl = viewModel.ReturnUrl, EmailAddress = signInResult.Value?.LoginHint };

            if (!ModelState.IsValid)
                return View(NewLoginViewModel());

            if (!signInResult.IsSuccess)
            {
                var signInErrors = signInResult.Errors;

                if (signInErrors.Contains(LoginUserErrors.UserNameOrPasswordIncorrect()))
                {
                    ModelState.Remove(nameof(LoginViewModel.Password));
                    ModelState.AddModelError(nameof(LoginViewModel.EmailAddress), SignInErrorMessage);
                    ModelState.AddModelError(nameof(LoginViewModel.Password), SignInErrorMessage);
                }

                if (!signInErrors.Contains(LoginUserErrors.UserIsDisabled()))
                    return View(NewLoginViewModel());

                var disabledErrorFormat = string.Format(
                    CultureInfo.CurrentCulture,
                    UserDisabledErrorMessageTemplate,
                    disabledErrorMessageSettings.EmailAddress,
                    disabledErrorMessageSettings.PhoneNumber);

                ModelState.AddModelError(nameof(LoginViewModel.DisabledError), disabledErrorFormat);

                return View(NewLoginViewModel());
            }

            var returnUrl = returnUri.ToString();

            // We can trust viewModel.ReturnUrl since GetAuthorizationContextAsync returned non-null
            if (signInResult.Value.IsTrustedReturnUrl)
                return Redirect(returnUrl);

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            LogoutRequest logoutRequest = await logoutService.GetLogoutRequestAsync(logoutId);

            await logoutService.SignOutAsync(logoutRequest);

            var redirectUrl = logoutRequest?.PostLogoutRedirectUri;

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = publicBrowseSettings.LogoutAddress.ToString();
            }

            return Redirect(redirectUrl);
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
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var resetToken = await passwordService.GeneratePasswordResetTokenAsync(viewModel.EmailAddress);
            if (resetToken is null)
                return RedirectToAction(nameof(ForgotPasswordLinkSent));

            await passwordService.SendResetEmailAsync(
                resetToken.User,
                passwordResetCallback.GetPasswordResetCallback(resetToken));

            return RedirectToAction(nameof(ForgotPasswordLinkSent));
        }

        [HttpGet]
        public IActionResult ForgotPasswordLinkSent()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            var isValid = await passwordService.IsValidPasswordResetTokenAsync(email, token);

            if (!isValid)
            {
                return RedirectToAction(nameof(ResetPasswordExpired));
            }

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var res = await passwordService.ResetPasswordAsync(viewModel.Email, viewModel.Token, viewModel.Password);
            if (res.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var invalidPasswordError = res.Errors.FirstOrDefault(error => error.Code == PasswordValidator.InvalidPasswordCode);
            if (invalidPasswordError is not null)
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), invalidPasswordError.Description);
                return View(viewModel);
            }

            var invalidTokenError = res.Errors.FirstOrDefault(error => error.Code == PasswordService.InvalidTokenCode);
            if (invalidTokenError is not null)
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

        private RedirectResult RedirectToPublicBrowseLogin()
        {
            return Redirect(publicBrowseSettings.LoginAddress.ToString());
        }
    }
}
