using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class ErrorControllerTests
    {
        [Test]
        public async Task Index_WithId_DisplaysErrorView()
        {
            var context = new TestContext();
            var res = await context.Controller.Index("testId") as ViewResult;
            res.Should().NotBeNull();
            res.ViewName.Should().Be("Error");
        }

        [Test]
        public async Task Index_WithId_LogsErrorMessage()
        {
            var context = new TestContext();

            await context.Controller.Index("testId");
            context.Logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) =>
                        o.ToString().Contains(context.ErrorMessage.ErrorDescription, StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        private class TestContext
        {
            public ErrorController Controller { get; }
            public Mock<IIdentityServerInteractionService> InteractionService { get; }
            public Mock<ILogger<ErrorController>> Logger { get; }
            public ErrorMessage ErrorMessage { get; }

            public TestContext()
            {
                ErrorMessage = new ErrorMessage {ErrorDescription = "This is the expected description"};

                this.InteractionService = new Mock<IIdentityServerInteractionService>();
                InteractionService.Setup(x => x.GetErrorContextAsync(It.IsAny<string>())).ReturnsAsync(ErrorMessage);

                this.Logger = new Mock<ILogger<ErrorController>>();
                this.Controller = new ErrorController(this.InteractionService.Object, this.Logger.Object);
            }
        }
    }
}
