using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class ErrorController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(
            IIdentityServerInteractionService interactionService,
            ILogger<ErrorController> logger)
        {
            _interactionService = interactionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string errorId)
        {
            var message = await _interactionService.GetErrorContextAsync(errorId);
            _logger.Log(LogLevel.Error, "Request for client {ClientId} failed with error code {Error}: {ErrorDescription}",
                message.ClientId,
                message.Error,
                message.ErrorDescription);
            return View("Error");
        }
    }
}
