using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    public sealed class ErrorControllerTests
    {
        [Test]
        public async Task Index_WithId_DisplaysErrorView()
        {
            using var controller = new ErrorControllerBuilder().Build();

            var result = await controller.Index("testId") as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

        [Test]
        public async Task Index_WithId_LogsErrorMessage()
        {
            var errorMessage = new ErrorMessage {ErrorDescription = "An error description"};
            var builder = new ErrorControllerBuilder().WithErrorMessage(errorMessage);
            using var controller = builder.Build();

            await controller.Index("testId");
            
            builder.Logger.Verify(
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
