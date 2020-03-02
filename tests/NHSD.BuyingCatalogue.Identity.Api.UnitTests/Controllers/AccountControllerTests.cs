using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
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
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.GetLogoutRequest(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            const string expectedLogoutId = null;
            var actual = await sut.Logout(expectedLogoutId);
            
            actual.Should().BeEquivalentTo(new RedirectResult(expectedLogoutRequest.PostLogoutRedirectUri));
        }

        [Test]
        public async Task Logout_WhenEmptyLogoutViewModel_ReturnsRedirectResult()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.GetLogoutRequest(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            LogoutViewModel expectedLogoutViewModel = new LogoutViewModel();
            var actual = await sut.Logout(expectedLogoutViewModel);
            
            actual.Should().BeEquivalentTo(new RedirectResult(expectedLogoutRequest.PostLogoutRedirectUri));
        }

        [Test]
        public async Task Logout_WhenEmptyLogoutViewModel_LogoutServiceSignOutCalledOnce()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.SignOutAsync(It.IsAny<LogoutRequest>()))
                .Returns(Task.CompletedTask);
            logoutServiceMock.Setup(x => x.GetLogoutRequest(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            LogoutViewModel expectedLogoutViewModel = new LogoutViewModel();
            await sut.Logout(expectedLogoutViewModel);

            logoutServiceMock.Verify(x => x.SignOutAsync(It.Is<LogoutRequest>(actual => actual.Equals(expectedLogoutRequest))), Times.Once);
        }

        [Test]
        public async Task Logout_WhenEmptyLogoutViewModel_LogoutServiceGetPostLogoutRedirectUriCalledOnce()
        {
            var expectedLogoutRequest = LogoutRequestBuilder
                .Create()
                .Build();

            var logoutServiceMock = new Mock<ILogoutService>();
            logoutServiceMock.Setup(x => x.GetLogoutRequest(It.IsAny<string>()))
                .ReturnsAsync(expectedLogoutRequest);

            using var sut = new AccountControllerBuilder()
                .WithLogoutService(logoutServiceMock.Object)
                .Build();

            await sut.Logout(new LogoutViewModel());

            logoutServiceMock.Verify(x => x.GetLogoutRequest(
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
