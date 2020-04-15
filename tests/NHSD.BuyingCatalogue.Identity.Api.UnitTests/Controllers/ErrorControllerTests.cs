using System;
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
            var errorMessage = new ErrorMessage { ErrorDescription = "An error description" };
            var interactionService = new Mock<IIdentityServerInteractionService>();
            interactionService.Setup(x => x.GetErrorContextAsync(It.IsAny<string>())).ReturnsAsync(errorMessage);
            var logger = new Mock<ILogger<ErrorController>>();

            using var controller = new ErrorController(interactionService.Object, logger.Object);

            var result = await controller.Index("testId") as ViewResult;
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

        [Test]
        public async Task Index_WithId_LogsErrorMessage()
        {
            var errorMessage = new ErrorMessage {ErrorDescription = "An error description"};
            var interactionService = new Mock<IIdentityServerInteractionService>();
            interactionService.Setup(x => x.GetErrorContextAsync(It.IsAny<string>())).ReturnsAsync(errorMessage);
            var logger = new Mock<ILogger<ErrorController>>();
            using var controller = new ErrorController(interactionService.Object, logger.Object);

            await controller.Index("testId");

            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) =>
                        o.ToString().Contains(errorMessage.ErrorDescription, StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
