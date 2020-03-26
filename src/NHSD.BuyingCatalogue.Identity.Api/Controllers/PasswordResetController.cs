using System;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.PasswordReset;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    [Route(@"Account")]
    public class PasswordResetController : Controller
    {
        [HttpGet]
        [Route(@"ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route(@"ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("LinkSent");
        }

        [HttpGet]
        [Route(@"LinkSent")]
        public IActionResult LinkSent()
        {
            return View();
        }
    }
}
