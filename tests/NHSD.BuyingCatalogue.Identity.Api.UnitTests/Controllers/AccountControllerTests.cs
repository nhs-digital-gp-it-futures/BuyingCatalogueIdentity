using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;
using NUnit.Framework;
using SignInResult = NHSD.BuyingCatalogue.Identity.Api.Services.SignInResult;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class AccountControllerTests
    {
        [Test]
        public void Error_ReturnsExpectedView()
        {
            using var controller = new AccountControllerBuilder().Build();

            var result = controller.Error() as ViewResult;

            Assert.NotNull(result);
            result.ViewName.Should().Be("Error");
        }

        [Test]
        public async Task Login_LoginViewModel_FailedSignIn_AddsValidationError()
        {
            using var controller = new AccountControllerBuilder()
                .WithSignInResult(new SignInResult(false))
                .Build();

            await controller.Login(new LoginViewModel());

            var modelState = controller.ModelState;

            modelState.IsValid.Should().BeFalse();
            modelState.Count.Should().Be(1);

            (string key, ModelStateEntry entry) = modelState.First();
            key.Should().Be(string.Empty);
            entry.Errors.Count.Should().Be(1);
            entry.Errors.First().ErrorMessage.Should().Be(AccountController.SignInErrorMessage);
        }

        [Test]
        public async Task Login_LoginViewModel_FailedSignIn_ReturnsExpectedView()
        {
            using var controller = new AccountControllerBuilder()
                .WithSignInResult(new SignInResult(false))
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
                .WithSignInResult(new SignInResult(false))
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
                .WithSignInResult(new SignInResult(false, loginHint: loginHint))
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
                .WithSignInResult(new SignInResult(true, true))
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
                .WithSignInResult(new SignInResult(true))
                .Build();

            var result = await controller.Login(
                new LoginViewModel { ReturnUrl = new Uri(rootUrl, UriKind.Relative) }) as LocalRedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(rootUrl);
        }

        [Test]
        public void Login_Uri_NullReturnUrl_ReturnsViewResultWithRootUrl()
        {
            var expectedUri = new Uri("~/", UriKind.Relative);

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
        public void Logout_WhenInvalidLogoutId_ShouldThrowArgumentNullException(string logoutId)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using var sut = new AccountControllerBuilder()
                    .Build();

                await sut.Logout(logoutId);
            });
        }
    }
}
