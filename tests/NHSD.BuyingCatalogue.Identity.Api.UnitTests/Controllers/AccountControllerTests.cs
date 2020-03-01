using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
	public sealed class AccountControllerTests
	{
        [Test]
        public async Task Logout_WhenLogoutIdIsNull_ReturnsRedirectResult()
        {
            const string expectedPostDirectUrl = "localhost";

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.SignOutAsync(It.IsAny<LogoutViewModel>()))
                .Returns(Task.CompletedTask);
            logoutServiceMock.Setup(x => x.GetPostLogoutRedirectUri(It.IsAny<string>()))
                .ReturnsAsync(expectedPostDirectUrl);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = null;
            var actual = await sut.Logout(expectedLogoutId);
            
            actual.Should().BeEquivalentTo(new RedirectResult(expectedPostDirectUrl));
        }

        [Test]
        public async Task Logout_WhenEmptyLogoutViewModel_ReturnsRedirectResult()
        {
            const string expectedPostDirectUrl = "localhost";

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.SignOutAsync(It.IsAny<LogoutViewModel>()))
                .Returns(Task.CompletedTask);
            logoutServiceMock.Setup(x => x.GetPostLogoutRedirectUri(It.IsAny<string>()))
                .ReturnsAsync(expectedPostDirectUrl);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            LogoutViewModel expectedLogoutViewModel = new LogoutViewModel();
            var actual = await sut.Logout(expectedLogoutViewModel);
            
            actual.Should().BeEquivalentTo(new RedirectResult(expectedPostDirectUrl));
        }

        [Test]
        public async Task Logout_WhenEmptyLogoutViewModel_LogoutServiceSignOutCalledOnce()
        {
            const string expectedPostDirectUrl = "localhost";

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.SignOutAsync(It.IsAny<LogoutViewModel>()))
                .Returns(Task.CompletedTask);
            logoutServiceMock.Setup(x => x.GetPostLogoutRedirectUri(It.IsAny<string>()))
                .ReturnsAsync(expectedPostDirectUrl);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            LogoutViewModel expectedLogoutViewModel = new LogoutViewModel();
            await sut.Logout(expectedLogoutViewModel);

            logoutServiceMock.Verify(x => x.SignOutAsync(It.Is<LogoutViewModel>(actual => actual.Equals(expectedLogoutViewModel))), Times.Once);
        }

        [Test]
        public async Task Logout_WhenEmptyLogoutViewModel_LogoutServiceGetPostLogoutRedirectUriCalledOnce()
        {
            const string expectedPostDirectUrl = "localhost";

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.SignOutAsync(It.IsAny<LogoutViewModel>()))
                .Returns(Task.CompletedTask);
            logoutServiceMock.Setup(x => x.GetPostLogoutRedirectUri(It.IsAny<string>()))
                .ReturnsAsync(expectedPostDirectUrl);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            await sut.Logout(new LogoutViewModel());

            logoutServiceMock.Verify(x => x.GetPostLogoutRedirectUri(
                It.Is<string>(actual => actual == null)), Times.Once);
        }

        [Test]
        public void Logout_WhenNullLogoutViewModel_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using var sut = new AccountControllerBuilder()
                    .Build();

                await sut.Logout((LogoutViewModel) null);
            });
        }
	}
}
