using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.PasswordReset;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    [Route(@"Account")]
    public class PasswordResetController : Controller
    {
        [HttpGet]
        [Route(@"ResetPassword")]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [Route(@"ResetPassword")]
        public IActionResult ResetPassword(PasswordResetViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));
            return RedirectToAction("ConfirmPasswordReset");
        }

        [HttpGet]
        [Route(@"ConfirmPasswordReset")]
        public IActionResult ConfirmPasswordReset()
        {
            return View();
        }
    }
}
