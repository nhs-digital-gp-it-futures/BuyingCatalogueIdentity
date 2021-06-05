#nullable enable
using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.SharedMocks;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HttpRequestExtensionsTests
    {
        [Test]
        public static void ShowCookieConsent_Gets_CallsCookieCollection()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(new Dictionary<string, string>()));

            httpRequest.Object.ShowCookieConsent(null);

            httpRequest.VerifyGet(h => h.Cookies);
        }

        [Test]
        public static void ShowCookieConsent_NoConsentCookiePresent_ReturnsTrue()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string> { { "some-cookie", "some-value" }, }));

            var actual = httpRequest.Object.ShowCookieConsent(null);

            actual.Should().BeTrue();
        }

        [Test]
        public static void ShowCookieConsent_ConsentCookieWithDateValueBeforePolicyDate_ReturnsTrue()
        {
            var policyDate = DateTime.Now.AddDays(-10);
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string>
                    {
                        {
                            Cookies.BuyingCatalogueConsent,
                            policyDate.AddHours(-4).ToCookieDataString()
                        },
                    }));

            var actual = httpRequest.Object.ShowCookieConsent(policyDate);

            actual.Should().BeTrue();
        }

        [Test]
        public static void ShowCookieConsent_ConsentCookiePresent_NoPolicDate_ReturnsFalse()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string>
                    {
                        {
                            Cookies.BuyingCatalogueConsent,
                            DateTime.Now.AddHours(-4).ToCookieDataString()
                        },
                    }));

            var actual = httpRequest.Object.ShowCookieConsent(null);

            actual.Should().BeFalse();
        }

        [Test]
        public static void ShowCookieConsent_CookieWithDateValueAfterPolicyDate_ReturnsFalse()
        {
            var policyDate = DateTime.Now.AddDays(-10);
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string>
                    {
                        {
                            Cookies.BuyingCatalogueConsent,
                            policyDate.AddHours(+1).ToCookieDataString()
                        },
                    }));

            var actual = httpRequest.Object.ShowCookieConsent(policyDate);

            actual.Should().BeFalse();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("some-value")]
        public static void ShowCookieConsent_CookieWithInvalidValue_ReturnsFalse(string invalid)
        {
            var policyDate = DateTime.Now.AddDays(-10);
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string> { { Cookies.BuyingCatalogueConsent, invalid }, }));

            var actual = httpRequest.Object.ShowCookieConsent(policyDate);

            actual.Should().BeFalse();
        }

        [Test]
        public static void ShowCookieConsent_PolicyDateAfterNow_ReturnsFalse()
        {
            var policyDate = DateTime.Now.AddMinutes(6);
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string>
                    {
                        {
                            Cookies.BuyingCatalogueConsent,
                            policyDate.AddDays(-23).ToCookieDataString()
                        },
                    }));

            var actual = httpRequest.Object.ShowCookieConsent(policyDate);

            httpRequest.VerifyGet(h => h.Cookies);
            actual.Should().BeFalse();
        }

        [Test]
        public static void ShowCookieConsent_PolicyDateIsNull_ReturnsFalse()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(
                    new Dictionary<string, string>
                    {
                        {
                            Cookies.BuyingCatalogueConsent,
                            DateTime.Now.AddDays(-23).ToCookieDataString()
                        },
                    }));

            var actual = httpRequest.Object.ShowCookieConsent(null);

            httpRequest.VerifyGet(h => h.Cookies);
            actual.Should().BeFalse();
        }

        [Test]
        public static void ShowCookieConsent_CookieNotPresent_ReturnsTrue()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.SetupGet(h => h.Cookies)
                .Returns(new MockRequestCookieCollection(new Dictionary<string, string>()));

            var actual = httpRequest.Object.ShowCookieConsent(null);

            httpRequest.VerifyGet(h => h.Cookies);
            actual.Should().BeTrue();
        }

        [Test]
        public static void ShowCookieConsent_HttpRequestIsNull_ReturnsFalse()
        {
            var actual = default(HttpRequest).ShowCookieConsent(null);

            actual.Should().BeFalse();
        }
    }
}
