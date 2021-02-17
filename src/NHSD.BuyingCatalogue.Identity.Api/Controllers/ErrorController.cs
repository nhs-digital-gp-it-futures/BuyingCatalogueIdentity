using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class ErrorController : Controller
    {
        private readonly IIdentityServerInteractionService interactionService;
        private readonly ILogger<ErrorController> logger;

        public ErrorController(
            IIdentityServerInteractionService interactionService,
            ILogger<ErrorController> logger)
        {
            this.interactionService = interactionService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string errorId)
        {
            var message = await interactionService.GetErrorContextAsync(errorId);
            logger.LogError(
                "Request for client {ClientId} failed with error code {Error}: {ErrorDescription}",
                message.ClientId,
                message.Error,
                message.ErrorDescription);

            return View("Error");
        }
    }
}
