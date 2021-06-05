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

            if (!buyingCatalogueCookiePolicyDate.HasValue)
                return false;

            return buyingCatalogueCookiePolicyDate.Value <= DateTime.Now
                && consentCookieValue.ExtractCookieCreationDate() is { } creationDate
                && creationDate < buyingCatalogueCookiePolicyDate.Value;
        }
    }
}
