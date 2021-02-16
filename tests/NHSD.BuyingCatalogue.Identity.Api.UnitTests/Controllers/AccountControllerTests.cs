﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Errors;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AccountControllerTests
    {
        [Test]
        public static async Task Login_LoginViewModel_FailedSignIn_AddsUsernameOrPasswordValidationError()
        {
            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails> { LoginUserErrors.UserNameOrPasswordIncorrect() }))
                .Build();

            await controller.Login(LoginViewModelBuilder.Create().Build());

            var modelState = controller.ModelState;

            modelState.IsValid.Should().BeFalse();
            modelState.Count.Should().Be(2);

            foreach ((_, ModelStateEntry entry) in modelState)
            {
                entry.Errors.Count.Should().Be(1);
                entry.Errors.First().ErrorMessage.Should().Be(AccountController.SignInErrorMessage);
            }
        }

        [Test]
        public static async Task Login_LoginViewModel_FailedSignIn_AddsDisabledValidationError()
        {
            const string email = "test@email.com";
            const string phoneNumber = "012345678901";

            var disabledSetting = new DisabledErrorMessageSettings
            {
                EmailAddress = email,
                PhoneNumber = phoneNumber,
            };

            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails> { LoginUserErrors.UserIsDisabled() }))
                .WithDisabledErrorMessageSetting(disabledSetting)
                .Build();

            await controller.Login(LoginViewModelBuilder.Create().Build());

            var modelState = controller.ModelState;

            modelState.IsValid.Should().BeFalse();
            modelState.Count.Should().Be(1);

            (string key, ModelStateEntry entry) = modelState.First();
            key.Should().Be(nameof(LoginViewModel.DisabledError));
            entry.Errors.Count.Should().Be(1);
            var expected = string.Format(
                CultureInfo.CurrentCulture,
                AccountController.UserDisabledErrorMessageTemplate,
                email,
                phoneNumber);

            entry.Errors.First().ErrorMessage.Should().Be(expected);
        }

        [Test]
        public static async Task Login_LoginViewModel_FailedSignIn_ReturnsExpectedView()
        {
            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails>()))
                .Build();

            var result = await controller.Login(LoginViewModelBuilder.Create().Build()) as ViewResult;

            Assert.NotNull(result);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
        }

        [Test]
        public static async Task Login_LoginViewModel_InvalidViewModel_WithoutEmailAddressAndPassword__ReturnsExpectedView()
        {
            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails>()))
                .Build();

            controller.ModelState.AddModelError(string.Empty, "Fake error!");

            var inputModel = LoginViewModelBuilder
                .Create()
                .Build();

            var result = await controller.Login(inputModel) as ViewResult;

            Assert.NotNull(result);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
            viewModel.ReturnUrl.Should().Be(inputModel.ReturnUrl);
            viewModel.Password.Should().BeNull();
            viewModel.EmailAddress.Should().BeNull();
        }

        [Test]
        public static async Task Login_LoginViewModel_InvalidViewModelWithValues_ReturnsExpectedView()
        {
            const string loginHint = "LoginHint";

            var uri = new Uri("http://www.foobar.com/");

            var inputModel = LoginViewModelBuilder
                .Create()
                .WithEmailAddress("NotLoginHint")
                .WithReturnUrl(uri)
                .WithPassword("Password")
                .Build();

            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(new Result<SignInResponse>(false, null, new SignInResponse(false, loginHint)))
                .Build();

            controller.ModelState.AddModelError(string.Empty, "Fake error!");

            var result = await controller.Login(inputModel) as ViewResult;
            Assert.NotNull(result);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
            viewModel.ReturnUrl.Should().BeEquivalentTo(uri);
            viewModel.Password.Should().BeNull();
            viewModel.EmailAddress.Should().BeEquivalentTo(loginHint);
        }

        [Test]
        public static void Login_LoginViewModel_NullViewModel_ThrowsException()
        {
            static async Task Login()
            {
                using var controller = AccountControllerBuilder
                    .Create()
                    .Build();

                await controller.Login((LoginViewModel)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Login);
        }

        [Test]
        public static async Task Login_LoginViewModel_SuccessfulSignInWithTrustedReturnUrl_ReturnsRedirectResult()
        {
            const string goodUrl = "https://www.realclient.co.uk/";

            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(new Result<SignInResponse>(true, null, new SignInResponse(true)))
                .Build();

            var inputModel = LoginViewModelBuilder
                .Create()
                .WithReturnUrl(new Uri(goodUrl))
                .Build();

            var result = await controller.Login(inputModel) as RedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(goodUrl);
        }

        [Test]
        public static async Task Login_LoginViewModel_SuccessfulSignInWithUntrustedReturnUrl_ReturnsLocalRedirectResult()
        {
            const string rootUrl = "~/";

            using var controller = AccountControllerBuilder
                .Create()
                .WithSignInResult(new Result<SignInResponse>(true, null, new SignInResponse()))
                .Build();

            LoginViewModel inputModel = LoginViewModelBuilder
                .Create()
                .WithReturnUrl(new Uri(rootUrl, UriKind.Relative))
                .Build();

            var result = await controller.Login(inputModel) as LocalRedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(rootUrl);
        }

        [Test]
        public static void Login_Uri_NullReturnUrl_ReturnsRedirectResult()
        {
            var publicBrowseSettings = new PublicBrowseSettings
            {
                BaseAddress = "https://public-prowse",
                LoginPath = "/some-login-path",
            };

            using var controller = AccountControllerBuilder
                .Create()
                .WithPublicBrowseSettings(publicBrowseSettings)
                .Build();

            var result = controller.Login((Uri)null) as RedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(publicBrowseSettings.LoginAddress.ToString());
        }

        [Test]
        public static void Login_Uri_WithReturnUrl_ReturnsViewResultWithReturnUrl()
        {
            var uri = new Uri("http://www.foobar.com/");

            using var controller = AccountControllerBuilder
                .Create()
                .Build();

            var result = controller.Login(uri) as ViewResult;

            Assert.NotNull(result);
            result.ViewData["ReturnUrl"].Should().Be(uri);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
            viewModel.ReturnUrl.Should().Be(uri);
        }

        [Test]
        public static async Task Logout_WhenLogoutIdIsNotNullOrEmpty_ReturnsRedirectResult()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(s => s.GetLogoutRequestAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = AccountControllerBuilder
                .Create()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = "123";
            var actual = await sut.Logout(expectedLogoutId);

            actual.Should().BeEquivalentTo(new RedirectResult(expectedLogoutRequest.PostLogoutRedirectUri));
        }

        [Test]
        public static async Task Logout_WhenLogoutIdIsNotNullOrEmpty_LogoutServiceSignOutCalledOnce()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(s => s.SignOutAsync(It.IsAny<LogoutRequest>())).Returns(Task.CompletedTask);
            logoutServiceMock
                .Setup(s => s.GetLogoutRequestAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = AccountControllerBuilder
                .Create()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = "123";
            await sut.Logout(expectedLogoutId);

            logoutServiceMock.Verify(s => s.SignOutAsync(It.Is<LogoutRequest>(actual => actual.Equals(expectedLogoutRequest))));
        }

        [Test]
        public static async Task Logout_WhenLogoutIdIsNotNullOrEmpty_LogoutServiceGetPostLogoutRedirectUriCalledOnce()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock
                .Setup(s => s.GetLogoutRequestAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = AccountControllerBuilder
                .Create()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = "123";
            await sut.Logout(expectedLogoutId);

            logoutServiceMock.Verify(
                s => s.GetLogoutRequestAsync(It.Is<string>(actual => expectedLogoutId.Equals(actual, StringComparison.Ordinal))));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static async Task Logout_WhenInvalidLogoutId_ShouldGoBackToBaseUrl(string logoutId)
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(s => s.GetLogoutRequestAsync(logoutId)).ReturnsAsync(expectedLogoutRequest);

            using var sut = AccountControllerBuilder
                .Create()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            var actual = await sut.Logout(logoutId);

            actual.Should().BeEquivalentTo(new RedirectResult(expectedLogoutRequest.PostLogoutRedirectUri));
        }

        [Test]
        public static void ForgotPassword_ForgotPasswordViewModel_NullViewModel_ThrowsException()
        {
            static async Task ForgotPassword()
            {
                using var controller = AccountControllerBuilder
                    .Create()
                    .Build();
                await controller.ForgotPassword((ForgotPasswordViewModel)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(ForgotPassword);
        }

        [Test]
        public static async Task ForgotPassword_ForgotPasswordViewModel_InvalidViewModel_ReturnsExpectedView()
        {
            using var controller = AccountControllerBuilder
                .Create()
                .Build();

            controller.ModelState.AddModelError(string.Empty, "Fake error!");

            var expectedViewModel = new ForgotPasswordViewModel();
            var result = await controller.ForgotPassword(expectedViewModel) as ViewResult;

            Assert.NotNull(result);
            var actualViewModel = result.Model;
            actualViewModel.Should().Be(expectedViewModel);
        }

        [Test]
        public static async Task ForgotPassword_ForgotPasswordViewModel_NullResetToken_RedirectedToExpectedAction()
        {
            using var controller = AccountControllerBuilder
                .Create()
                .WithResetToken(null)
                .Build();

            var result = await controller.ForgotPassword(
                new ForgotPasswordViewModel { EmailAddress = "a@b.com" }) as RedirectToActionResult;

            Assert.NotNull(result);
            result.ActionName.Should().Be(nameof(AccountController.ForgotPasswordLinkSent));
        }

        [Test]
        public static async Task ForgotPassword_ForgotPasswordViewModel_WithResetToken_SendsResetEmail()
        {
            var callbackCount = 0;
            ApplicationUser actualUser = null;
            Uri actualCallback = null;

            void EmailCallback(ApplicationUser user, Uri callback)
            {
                callbackCount++;
                actualUser = user;
                actualCallback = callback;
            }

            const string expectedCallback = "https://identity/account/resetPassword?token=1234&emailAddress=a@b.com";
            var expectedUser = ApplicationUserBuilder.Create().Build();

            using var controller = AccountControllerBuilder
                .Create()
                .WithResetEmailCallback(EmailCallback)
                .WithResetToken(new PasswordResetToken("token", expectedUser))
                .WithScheme(HttpScheme.Https.ToString())
                .WithResetCallbackUrl(expectedCallback)
                .Build();

            await controller.ForgotPassword(new ForgotPasswordViewModel { EmailAddress = "a@b.com" });

            callbackCount.Should().Be(1);
            actualUser.Should().Be(expectedUser);
            actualCallback.Should().Be(expectedCallback);
        }

        [Test]
        public static async Task ForgotPassword_ForgotPasswordViewModel_WithResetToken_RedirectedToExpectedAction()
        {
            using var controller = AccountControllerBuilder
                .Create()
                .WithResetToken(new PasswordResetToken("token", ApplicationUserBuilder.Create().Build()))
                .WithScheme(HttpScheme.Https.ToString())
                .WithResetCallbackUrl("https://identity/account/action")
                .Build();

            var result = await controller.ForgotPassword(
                new ForgotPasswordViewModel { EmailAddress = "a@b.com" }) as RedirectToActionResult;

            Assert.NotNull(result);
            result.ActionName.Should().Be(nameof(AccountController.ForgotPasswordLinkSent));
        }

        [Test]
        public static async Task ResetPassword_String_String_ValidToken_ReturnsExpectedView()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";

            using var controller = AccountControllerBuilder
                .Create()
                .WithIsValidPasswordResetTokenResult()
                .Build();

            var result = await controller.ResetPassword(email, expectedToken) as ViewResult;
            Assert.NotNull(result);

            var model = result.Model as ResetPasswordViewModel;
            Assert.NotNull(model);

            model.Email.Should().Be(email);
            model.Token.Should().Be(expectedToken);
        }

        [Test]
        public static async Task ResetPassword_String_String_InvalidToken_ReturnsExpectedAction()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";

            using var controller = AccountControllerBuilder
                .Create()
                .Build();

            var result = await controller.ResetPassword(email, expectedToken) as RedirectToActionResult;
            Assert.NotNull(result);

            result.ActionName.Should().Be(nameof(AccountController.ResetPasswordExpired));
        }

        [Test]
        public static async Task ResetPassword_InvalidPassword_ReturnsExpectedView()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";
            var viewModel = new ResetPasswordViewModel { Email = email, Token = expectedToken };
            var identityResult = IdentityResult.Failed(new IdentityError
            {
                Code = PasswordValidator.InvalidPasswordCode,
                Description = PasswordValidator.PasswordConditionsNotMet,
            });

            using var controller = AccountControllerBuilder
                .Create()
                .WithPasswordResetResult(identityResult)
                .Build();

            var result = (await controller.ResetPassword(viewModel)) as ViewResult;
            Assert.NotNull(result);

            var model = result.Model as ResetPasswordViewModel;
            Assert.NotNull(model);

            model.Email.Should().Be(email);
            model.Token.Should().Be(expectedToken);
        }

        [Test]
        public static async Task ResetPassword_InvalidToken_RedirectsToPasswordExpired()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";
            var viewModel = new ResetPasswordViewModel { Email = email, Token = expectedToken };
            var identityResult = IdentityResult.Failed(new IdentityError
            {
                Code = PasswordService.InvalidTokenCode,
            });
            using var controller = AccountControllerBuilder
                .Create()
                .WithPasswordResetResult(identityResult)
                .Build();

            var result = await controller.ResetPassword(viewModel) as RedirectToActionResult;

            Assert.NotNull(result);
            result.ActionName.Should().Be(nameof(AccountController.ResetPasswordExpired));
        }

        [Test]
        public static void ResetPassword_UnexpectedError_ThrowsException()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";

            static async Task ForgotPassword()
            {
                var viewModel = new ResetPasswordViewModel { Email = email, Token = expectedToken };
                var identityResult = IdentityResult.Failed(new IdentityError
                {
                    Code = "SomethingWeirdHappened",
                });

                using var controller = AccountControllerBuilder
                    .Create()
                    .WithPasswordResetResult(identityResult)
                    .Build();

                await controller.ResetPassword(viewModel);
            }

            Assert.ThrowsAsync<InvalidOperationException>(ForgotPassword);
        }
    }
}
