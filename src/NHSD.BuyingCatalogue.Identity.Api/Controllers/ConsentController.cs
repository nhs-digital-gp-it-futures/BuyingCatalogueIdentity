using System;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Consent;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Controllers
{
    public sealed class ConsentController : Controller
    {
        private readonly IAgreementConsentService consentService;
        private readonly CookieExpirationSettings cookieExpiration;

        public ConsentController(IAgreementConsentService consentService, CookieExpirationSettings cookieExpiration)
        {
            this.consentService = consentService ?? throw new ArgumentNullException(nameof(consentService));
            this.cookieExpiration = cookieExpiration ?? throw new ArgumentNullException(nameof(cookieExpiration));
        }

        [HttpGet]
        public async Task<IActionResult> Index(Uri returnUrl)
        {
            if (returnUrl is null)
                throw new ArgumentNullException(nameof(returnUrl));

            if (await consentService.IsValidReturnUrl(returnUrl))
                return View(new ConsentViewModel { ReturnUrl = returnUrl });

            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentViewModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var returnUrl = model.ReturnUrl;

            var result = await consentService.GrantConsent(returnUrl, User.GetSubjectId());
            if (result.IsSuccess)
                return Redirect(returnUrl.ToString());

            return View("Error");
        }

        [HttpGet]
        public IActionResult DismissCookieBanner()
        {
            Response.Cookies.Append(
                Cookies.BuyingCatalogueConsent,
                DateTime.Now.ToCookieDataString(),
                new CookieOptions { Expires = DateTime.Now.Add(cookieExpiration.ConsentExpiration), });

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
    }
}
