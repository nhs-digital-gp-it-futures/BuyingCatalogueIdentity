using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public sealed class AuthenticateController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public AuthenticateController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginViewModel viewModel)
        {
            viewModel.ThrowIfNull();

            var context = await _interaction.GetAuthorizationContextAsync(viewModel.ReturnUrl.AbsoluteUri);

            var user = TestUsers.Users
                .FirstOrDefault(usr => usr.Password == viewModel.Password && usr.Username == viewModel.Username);

            if (user != null && context != null)
            {
                await HttpContext.SignInAsync(user.SubjectId, user.Username);
                return new JsonResult(new { RedirectUrl = viewModel.ReturnUrl, IsOk = true });
            }

            return Unauthorized();
        }
    }
}
