using System;
using Microsoft.AspNetCore.Http;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool ShowCookieConsent(this HttpRequest httpRequest, DateTime? buyingCatalogueCookiePolicyDate)
        {
            return httpRequest is not null &&
                (!httpRequest.Cookies.TryGetValue(
                        Cookies.BuyingCatalogueConsent,
                        out var consentCookieValue) ||
                    (buyingCatalogueCookiePolicyDate.HasValue
                        && buyingCatalogueCookiePolicyDate.Value < DateTime.Now
                        && DateTime.TryParse(consentCookieValue, out var cookieCreationDate)
                        && cookieCreationDate < buyingCatalogueCookiePolicyDate.Value));
        }
    }
}
