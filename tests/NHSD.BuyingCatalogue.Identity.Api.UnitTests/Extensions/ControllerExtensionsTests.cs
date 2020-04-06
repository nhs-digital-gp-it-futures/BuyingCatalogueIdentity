using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Extensions
{
    [TestFixture]
    internal sealed class ControllerExtensionsTests
    {
        [Test]
        public void Action_WithControllerSuffix_InvokesExpectedUrlAction()
        {
            const string expectedAction = nameof(AccountController.ResetPassword);
            const string expectedScheme = "scheme";

            var values = new { Token = "Token", Email = "Email" };
            UrlActionContext actualActionContext = null;

            using var controller = new AccountControllerBuilder()
                .WithUrlActionCallback(a => actualActionContext = a)
                .WithScheme(expectedScheme)
                .Build();

            controller.Action(expectedAction, values);

            Assert.NotNull(actualActionContext);
            actualActionContext.Action.Should().Be(expectedAction);
            actualActionContext.Controller.Should().Be("Account");
            actualActionContext.Values.Should().Be(values);
        }

        [Test]
        public void Action_WithoutControllerSuffix_InvokesExpectedUrlAction()
        {
            const string expectedAction = nameof(Test.ResetPassword);
            const string expectedScheme = "scheme";

            var values = new { Token = "Token", Email = "Email" };
            UrlActionContext actualActionContext = null;

            using var controller = new Test(a => actualActionContext = a, expectedScheme);

            controller.Action(expectedAction, values);

            Assert.NotNull(actualActionContext);
            actualActionContext.Action.Should().Be(expectedAction);
            actualActionContext.Controller.Should().Be("Test");
            actualActionContext.Values.Should().Be(values);
        }

        private class Test : Controller
        {
            internal Test(Action<UrlActionContext> actionCallback, string scheme)
            {
                var mockRequest = new Mock<HttpRequest>();
                mockRequest.Setup(r => r.Scheme).Returns(scheme);

                var mockHttpContext = new Mock<HttpContext>();
                mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

                var mockUrlHelper = new Mock<IUrlHelper>();
                mockUrlHelper.Setup(h => h.Action(It.IsNotNull<UrlActionContext>()))
                    .Callback(actionCallback);

                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object };
                Url = mockUrlHelper.Object;
            }

            internal static void ResetPassword()
            {
            }
        }
    }
}
