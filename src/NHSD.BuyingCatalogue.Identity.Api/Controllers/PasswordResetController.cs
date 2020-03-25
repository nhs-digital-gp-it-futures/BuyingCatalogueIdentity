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
        public IActionResult ResetPassword(Uri returnUrl)
        {
            returnUrl.ThrowIfNull();
            var viewModel = new PasswordResetViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(viewModel);
        }

        [HttpPost]
        [Route(@"ResetPassword")]
        public IActionResult ResetPassword(PasswordResetViewModel viewModel)
        {
            viewModel.ThrowIfNull(nameof(viewModel));
            viewModel.ReturnUrl.ThrowIfNull();
            return RedirectToAction("LinkSent", new { returnUrl = viewModel.ReturnUrl });
        }

        [HttpGet]
        [Route(@"LinkSent")]
        public IActionResult LinkSent(Uri returnUrl)
        {
            returnUrl.ThrowIfNull();
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
