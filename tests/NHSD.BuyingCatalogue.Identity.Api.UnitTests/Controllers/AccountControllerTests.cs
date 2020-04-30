using System;
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
    public sealed class AccountControllerTests
    {
        [Test]
        public async Task Login_LoginViewModel_FailedSignIn_AddsUsernameOrPasswordValidationError()
        {
            using var controller = new AccountControllerBuilder()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails> { LoginUserErrors.UserNameOrPasswordIncorrect() }))
                .Build();

            await controller.Login(new LoginViewModel());

            var modelState = controller.ModelState;

            modelState.IsValid.Should().BeFalse();
            modelState.Count.Should().Be(2);

            foreach ((string key, ModelStateEntry entry) in modelState)
            {
                entry.Errors.Count.Should().Be(1);
                entry.Errors.First().ErrorMessage.Should().Be(AccountController.SignInErrorMessage);
            }
        }

        [Test]
        public async Task Login_LoginViewModel_FailedSignIn_AddsDisabledValidationError()
        {
            const string email = "test@email.com";
            const string phoneNumber = "012345678901";

            var disabledSetting = new DisabledErrorMessageSettings()
            {
                EmailAddress = email,
                PhoneNumber = phoneNumber
            };

            using var controller = new AccountControllerBuilder()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails> { LoginUserErrors.UserIsDisabled() }))
                .WithDisabledErrorMessageSetting(disabledSetting)
                .Build();

            await controller.Login(new LoginViewModel());

            var modelState = controller.ModelState;

            modelState.IsValid.Should().BeFalse();
            modelState.Count.Should().Be(1);

            (string key, ModelStateEntry entry) = modelState.First();
            key.Should().Be(nameof(LoginViewModel.DisabledError));
            entry.Errors.Count.Should().Be(1);
            var expected = string.Format(CultureInfo.CurrentCulture, AccountController.UserDisabledErrorMessageTemplate, email,
            phoneNumber);
            entry.Errors.First().ErrorMessage.Should().Be(expected);
        }

        [Test]
        public async Task Login_LoginViewModel_FailedSignIn_ReturnsExpectedView()
        {
            using var controller = new AccountControllerBuilder()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails>()))
                .Build();

            var result = await controller.Login(new LoginViewModel()) as ViewResult;

            Assert.NotNull(result);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
        }

        [Test]
        public async Task Login_LoginViewModel_InvalidViewModelWithoutAnyValues_ReturnsExpectedView()
        {
            using var controller = new AccountControllerBuilder()
                .WithSignInResult(Result.Failure<SignInResponse>(new List<ErrorDetails>()))
                .Build();

            controller.ModelState.AddModelError(string.Empty, "Fake error!");

            var result = await controller.Login(new LoginViewModel()) as ViewResult;

            Assert.NotNull(result);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
            viewModel.ReturnUrl.Should().BeNull();
            viewModel.Password.Should().BeNull();
            viewModel.EmailAddress.Should().BeNull();
        }

        [Test]
        public async Task Login_LoginViewModel_InvalidViewModelWithValues_ReturnsExpectedView()
        {
            const string loginHint = "LoginHint";

            var uri = new Uri("http://www.foobar.com/");

            var inputModel = new LoginViewModel
            {
                Password = "Password",
                ReturnUrl = uri,
                EmailAddress = "NotLoginHint",
            };

            using var controller = new AccountControllerBuilder()
                .WithSignInResult(new Result<SignInResponse>(false, null, new SignInResponse(false, loginHint: loginHint)))
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
        public void Login_LoginViewModel_NullViewModel_ThrowsException()
        {
            static async Task Login()
            {
                using var controller = new AccountControllerBuilder().Build();
                await controller.Login((LoginViewModel)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Login);
        }

        [Test]
        public async Task Login_LoginViewModel_SuccessfulSignInWithTrustedReturnUrl_ReturnsRedirectResult()
        {
            const string goodUrl = "https://www.realclient.co.uk/";

            using var controller = new AccountControllerBuilder()
                .WithSignInResult(new Result<SignInResponse>(true, null, new SignInResponse(true)))
                .Build();

            var result = await controller.Login(
                new LoginViewModel { ReturnUrl = new Uri(goodUrl) }) as RedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(goodUrl);
        }

        [Test]
        public async Task Login_LoginViewModel_SuccessfulSignInWithUntrustedReturnUrl_ReturnsLocalRedirectResult()
        {
            const string rootUrl = "~/";

            using var controller = new AccountControllerBuilder()
                .WithSignInResult(new Result<SignInResponse>(true, null, new SignInResponse()))
                .Build();

            var result = await controller.Login(
                new LoginViewModel { ReturnUrl = new Uri(rootUrl, UriKind.Relative) }) as LocalRedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(rootUrl);
        }

        [Test]
        public void Login_Uri_NullReturnUrl_ReturnsViewResultWithRootUrl()
        {
            var expectedUri = new Uri("/", UriKind.Relative);

            using var controller = new AccountControllerBuilder().Build();

            var result = controller.Login((Uri)null) as ViewResult;

            Assert.NotNull(result);
            result.ViewData["ReturnUrl"].Should().BeEquivalentTo(expectedUri);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
            viewModel.ReturnUrl.Should().BeEquivalentTo(expectedUri);
        }

        [Test]
        public void Login_Uri_WithReturnUrl_ReturnsViewResultWithReturnUrl()
        {
            var uri = new Uri("http://www.foobar.com/");

            using var controller = new AccountControllerBuilder().Build();

            var result = controller.Login(uri) as ViewResult;

            Assert.NotNull(result);
            result.ViewData["ReturnUrl"].Should().Be(uri);

            var viewModel = result.Model as LoginViewModel;
            Assert.NotNull(viewModel);
            viewModel.ReturnUrl.Should().Be(uri);
        }

        [Test]
        public async Task Logout_WhenLogoutIdIsNotNullOrEmpty_ReturnsRedirectResult()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.GetLogoutRequestAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = "123";
            var actual = await sut.Logout(expectedLogoutId);

            actual.Should().BeEquivalentTo(new RedirectResult(expectedLogoutRequest.PostLogoutRedirectUri));
        }

        [Test]
        public async Task Logout_WhenLogoutIdIsNotNullOrEmpty_LogoutServiceSignOutCalledOnce()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.SignOutAsync(It.IsAny<LogoutRequest>()))
                .Returns(Task.CompletedTask);
            logoutServiceMock.Setup(x => x.GetLogoutRequestAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = "123";
            await sut.Logout(expectedLogoutId);

            logoutServiceMock.Verify(x => x.SignOutAsync(It.Is<LogoutRequest>(actual => actual.Equals(expectedLogoutRequest))), Times.Once);
        }

        [Test]
        public async Task Logout_WhenLogoutIdIsNotNullOrEmpty_LogoutServiceGetPostLogoutRedirectUriCalledOnce()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.GetLogoutRequestAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = "123";
            await sut.Logout(expectedLogoutId);

            logoutServiceMock.Verify(x => x.GetLogoutRequestAsync(
                It.Is<string>(actual => expectedLogoutId.Equals(actual, StringComparison.Ordinal))), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public async Task Logout_WhenInvalidLogoutId_ShouldGoBackToBaseUrl(string logoutId)
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.GetLogoutRequestAsync(logoutId))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            var actual = await sut.Logout(logoutId);

            actual.Should().BeEquivalentTo(new RedirectResult(expectedLogoutRequest.PostLogoutRedirectUri));
        }

        [Test]
        public void ForgotPassword_ForgotPasswordViewModel_NullViewModel_ThrowsException()
        {
            static async Task ForgotPassword()
            {
                using var controller = new AccountControllerBuilder().Build();
                await controller.ForgotPassword((ForgotPasswordViewModel)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(ForgotPassword);
        }

        [Test]
        public async Task ForgotPassword_ForgotPasswordViewModel_InvalidViewModel_ReturnsExpectedView()
        {
            using var controller = new AccountControllerBuilder()
                .Build();

            controller.ModelState.AddModelError(string.Empty, "Fake error!");

            var expectedViewModel = new ForgotPasswordViewModel();
            var result = await controller.ForgotPassword(expectedViewModel) as ViewResult;

            Assert.NotNull(result);
            var actualViewModel = result.Model;
            actualViewModel.Should().Be(expectedViewModel);
        }

        [Test]
        public async Task ForgotPassword_ForgotPasswordViewModel_NullResetToken_RedirectedToExpectedAction()
        {
            using var controller = new AccountControllerBuilder()
                .WithResetToken(null)
                .Build();

            var result = await controller.ForgotPassword(
                new ForgotPasswordViewModel { EmailAddress = "a@b.com" }) as RedirectToActionResult;

            Assert.NotNull(result);
            result.ActionName.Should().Be(nameof(AccountController.ForgotPasswordLinkSent));
        }

        [Test]
        public async Task ForgotPassword_ForgotPasswordViewModel_WithResetToken_SendsResetEmail()
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

            using var controller = new AccountControllerBuilder()
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
        public async Task ForgotPassword_ForgotPasswordViewModel_WithResetToken_RedirectedToExpectedAction()
        {
            using var controller = new AccountControllerBuilder()
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
        public async Task ResetPassword_String_String_ValidToken_ReturnsExpectedView()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";

            using var controller = new AccountControllerBuilder()
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
        public async Task ResetPassword_String_String_InvalidToken_ReturnsExpectedView()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";

            using var controller = new AccountControllerBuilder()
                .Build();

            var result = await controller.ResetPassword(email, expectedToken) as ViewResult;
            Assert.NotNull(result);

            result.ViewName.Should().Be(nameof(AccountController.ResetPasswordExpired));
        }

        [Test]
        public async Task ResetPassword_InvalidPassword_ReturnsExpectedView()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";
            var viewModel = new ResetPasswordViewModel { Email = email, Token = expectedToken };
            var identityResult = IdentityResult.Failed(new IdentityError
            {
                Code = PasswordValidator.InvalidPasswordCode,
                Description = PasswordValidator.PasswordConditionsNotMet
            });

            using var controller = new AccountControllerBuilder().WithPasswordResetResult(identityResult).Build();

            var result = (await controller.ResetPassword(viewModel)) as ViewResult;
            Assert.NotNull(result);

            var model = result.Model as ResetPasswordViewModel;
            Assert.NotNull(model);

            model.Email.Should().Be(email);
            model.Token.Should().Be(expectedToken);
        }

        [Test]
        public async Task ResetPassword_InvalidToken_RedirectsToPasswordExpired()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";
            var viewModel = new ResetPasswordViewModel { Email = email, Token = expectedToken };
            var identityResult = IdentityResult.Failed(new IdentityError
            {
                Code = PasswordService.InvalidTokenCode
            });
            using var controller = new AccountControllerBuilder().WithPasswordResetResult(identityResult).Build();

            var result = await controller.ResetPassword(viewModel) as RedirectToActionResult;

            Assert.NotNull(result);
            result.ActionName.Should().Be(nameof(AccountController.ResetPasswordExpired));
        }

        [Test]
        public void ResetPassword_UnexpectedError_ThrowsException()
        {
            const string email = "a@b.test";
            const string expectedToken = "TokenMcToken";

            static async Task ForgotPassword()
            {
                var viewModel = new ResetPasswordViewModel { Email = email, Token = expectedToken };
                var identityResult = IdentityResult.Failed(new IdentityError
                {
                    Code = "SomethingWeirdHappened"
                });

                using var controller = new AccountControllerBuilder().WithPasswordResetResult(identityResult).Build();
                await controller.ResetPassword(viewModel);
            }
            Assert.ThrowsAsync<InvalidOperationException>(ForgotPassword);
        }
    }
}
