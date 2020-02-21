using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class AuthenticateController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public AuthenticateController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore)
        {
            _interaction = interaction;
            _clientStore = clientStore;
        }

        [HttpGet]
        public async Task<IActionResult> Login(Uri returnUrl)
        {
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl?.AbsoluteUri);
            if (context?.IdP != null)
            {
                throw new NotImplementedException("External login is not implemented!");
            }

            LoginViewModel loginViewModel = await BuildLoginViewModelAsync(returnUrl, context);

            ViewData["ReturnUrl"] = returnUrl;

            return View(loginViewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> Login([FromBody]LoginViewModel viewModel)
        //{
        //    viewModel.ThrowIfNull();

        //    var context = await _interaction.GetAuthorizationContextAsync(viewModel.ReturnUrl.AbsoluteUri);

        //    var user = TestUsers.Users
        //        .FirstOrDefault(usr => usr.Password == viewModel.Password && usr.Username == viewModel.Username);

        //    if (user != null && context != null)
        //    {
        //        await HttpContext.SignInAsync(user.SubjectId, user.Username);
        //        return new JsonResult(new { RedirectUrl = viewModel.ReturnUrl, IsOk = true });
        //    }

        //    return Unauthorized();
        //}

        [HttpGet]
        public async Task<IActionResult> Error(string errorId)
        {
            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            
            return Ok(message);
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(Uri returnUrl, AuthorizationRequest context)
        {
            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                }
            }

            return new LoginViewModel
            {
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
            };
        }
    }
}
