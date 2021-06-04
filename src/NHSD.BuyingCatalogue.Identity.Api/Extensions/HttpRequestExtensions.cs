using System;
using Microsoft.AspNetCore.Http;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool ShowCookieConsent(this HttpRequest httpRequest, DateTime? buyingCatalogueCookiePolicyDate)
        {
            if (httpRequest is null)
                return false;

            if (!httpRequest.Cookies.TryGetValue(Cookies.BuyingCatalogueConsent, out var consentCookieValue))
                return true;

            return buyingCatalogueCookiePolicyDate.HasValue
                && buyingCatalogueCookiePolicyDate.Value < DateTime.Now
                && DateTime.TryParse(consentCookieValue, out var cookieCreationDate)
                && cookieCreationDate < buyingCatalogueCookiePolicyDate.Value;

            // return (!httpRequest.Cookies.ContainsKey(Cookies.BuyingCatalogueConsent)
            //     || (buyingCatalogueCookiePolicyDate.HasValue
            //         && buyingCatalogueCookiePolicyDate.Value < DateTime.Now
            //         && httpRequest.Cookies.TryGetValue(Cookies.BuyingCatalogueConsent, out var buyingConsentCookie)
            //         && DateTime.TryParse(buyingConsentCookie, out var cookieCreationDate)
            //         && cookieCreationDate < buyingCatalogueCookiePolicyDate));
        }
    }
}
