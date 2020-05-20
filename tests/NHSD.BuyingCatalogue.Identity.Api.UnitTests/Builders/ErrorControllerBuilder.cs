using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ErrorControllerBuilder
    {
        public Mock<IIdentityServerInteractionService> InteractionService { get; }
        public Mock<ILogger<ErrorController>> Logger { get; }
        public ErrorMessage ErrorMessage { get; private set; }

        public ErrorControllerBuilder()
        {
            ErrorMessage = new ErrorMessage {ErrorDescription = "Default description"};

            InteractionService = new Mock<IIdentityServerInteractionService>();
            InteractionService.Setup(x => x.GetErrorContextAsync(It.IsAny<string>())).ReturnsAsync(ErrorMessage);

            Logger = new Mock<ILogger<ErrorController>>();
        }

        internal ErrorControllerBuilder WithErrorMessage(ErrorMessage message)
        {
            ErrorMessage = message;
            InteractionService.Setup(x => x.GetErrorContextAsync(It.IsAny<string>())).ReturnsAsync(message);
            return this;
        }

        public ErrorController Build()
        {
            return new ErrorController(InteractionService.Object, Logger.Object);
        }
    }
}
