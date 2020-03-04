using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
	public sealed class AccountControllerTests
	{
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
