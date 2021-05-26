using Microsoft.AspNetCore.Http;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool ShowCookieConsent(this HttpRequest httpRequest) =>
            httpRequest is not null && !httpRequest.Cookies.ContainsKey(Cookies.BuyingCatalogueConsent);
    }
}
