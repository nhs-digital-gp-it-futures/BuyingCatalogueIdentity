using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HttpRequestExtensionsTests
    {
        [Test]
        public static void ShowCookieConsent_CookiePresent_ReturnsFalse()
        {
            var httpRequest = new Mock<HttpRequest>();
            var cookies = new Mock<IRequestCookieCollection>();
            cookies.Setup(c => c.ContainsKey(Cookies.BuyingCatalogueConsent))
                .Returns(true);
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(cookies.Object);

            var actual = httpRequest.Object.ShowCookieConsent();

            cookies.Verify(c => c.ContainsKey(Cookies.BuyingCatalogueConsent));
            actual.Should().BeFalse();
        }

        [Test]
        public static void ShowCookieConsent_CookieNotPresent_ReturnsTrue()
        {
            var httpRequest = new Mock<HttpRequest>();
            var cookies = new Mock<IRequestCookieCollection>();
            cookies.Setup(c => c.ContainsKey(Cookies.BuyingCatalogueConsent))
                .Returns(false);
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(cookies.Object);

            var actual = httpRequest.Object.ShowCookieConsent();

            cookies.Verify(c => c.ContainsKey(Cookies.BuyingCatalogueConsent));
            actual.Should().BeTrue();
        }

        [Test]
        public static void ShowCookieConsent_HttpRequestIsNull_ReturnsFalse()
        {
            var actual = default(HttpRequest).ShowCookieConsent();

            actual.Should().BeFalse();
        }
    }
}
