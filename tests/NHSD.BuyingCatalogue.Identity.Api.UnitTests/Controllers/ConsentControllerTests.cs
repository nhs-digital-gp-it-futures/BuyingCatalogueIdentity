using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Consent;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ConsentControllerTests
    {
        [Test]
        public static void Constructor_NullConsentService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ConsentController(null, new Settings.CookieExpirationSettings()));
        }

        [Test]
        public static void Index_Uri_NullReturnUrl_ThrowsException()
        {
            static async Task Index()
            {
                using var controller = new ConsentController(Mock.Of<IAgreementConsentService>(), new Settings.CookieExpirationSettings());
                await controller.Index((Uri)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Index);
        }

        [Test]
        public static async Task Index_Uri_ValidReturnUrl_ReturnsConsentView()
        {
            var returnUrl = new Uri("http://www.goodurl.co.uk/");

            var mockConsentService = new Mock<IAgreementConsentService>();
            mockConsentService.Setup(c => c.IsValidReturnUrl(It.IsNotNull<Uri>()))
                .ReturnsAsync(true);

            using var controller = new ConsentController(mockConsentService.Object, new Settings.CookieExpirationSettings());

            var result = await controller.Index(returnUrl) as ViewResult;

            Assert.NotNull(result);
            result.Model.Should().BeEquivalentTo(new ConsentViewModel { ReturnUrl = returnUrl });
        }

        [Test]
        public static async Task Index_Uri_BadReturnUrl_ReturnsErrorView()
        {
            using var controller = new ConsentController(Mock.Of<IAgreementConsentService>(), new Settings.CookieExpirationSettings());

            var result = await controller.Index(new Uri("http://www.badurl.co.uk/")) as ViewResult;

            Assert.NotNull(result);
            result.ViewName.Should().Be("Error");
        }

        [Test]
        public static void Index_ConsentViewModel_NullModel_ThrowsException()
        {
            static async Task Index()
            {
                using var controller = new ConsentController(Mock.Of<IAgreementConsentService>(), new Settings.CookieExpirationSettings());
                await controller.Index((ConsentViewModel)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Index);
        }

        [Test]
        public static async Task Index_ConsentViewModel_InvalidModelState_ReturnsConsentView()
        {
            var expectedModel = new ConsentViewModel();

            using var controller = new ConsentController(Mock.Of<IAgreementConsentService>(), new Settings.CookieExpirationSettings());
            controller.ModelState.AddModelError("Test", "Test");

            var result = await controller.Index(expectedModel) as ViewResult;

            Assert.NotNull(result);
            result.Model.Should().Be(expectedModel);
        }

        [Test]
        public static async Task Index_ConsentViewModel_ValidReturnUrlAndModel_RedirectsToReturnUrl()
        {
            const string subjectId = "SubjectId";
            var returnUrl = new Uri("http://www.goodurl.co.uk/");

            var mockConsentService = new Mock<IAgreementConsentService>();
            mockConsentService
                .Setup(c => c.GrantConsent(It.Is<Uri>(u => u == returnUrl), It.Is<string>(s => s.Equals(subjectId, StringComparison.Ordinal))))
                .ReturnsAsync(Result.Success(returnUrl));

            using var controller = new ConsentController(mockConsentService.Object, new Settings.CookieExpirationSettings())
            {
                ControllerContext = ControllerContext(subjectId),
            };

            var result = await controller.Index(
                new ConsentViewModel { ReturnUrl = returnUrl }) as RedirectResult;

            Assert.NotNull(result);
            result.Url.Should().Be(returnUrl.ToString());
        }

        [Test]
        public static async Task Index_ConsentViewModel_BadReturnUrl_ReturnsErrorView()
        {
            var mockConsentService = new Mock<IAgreementConsentService>();
            mockConsentService.Setup(c => c.GrantConsent(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<Uri>());

            using var controller = new ConsentController(mockConsentService.Object, new Settings.CookieExpirationSettings())
            {
                ControllerContext = ControllerContext(string.Empty),
            };

            var result = await controller.Index(new ConsentViewModel()) as ViewResult;

            Assert.NotNull(result);
            result.ViewName.Should().Be("Error");
        }

        [Test]
        public static void DismissCookieBanner_AllowAnonymousAttribute_Present()
        {
            typeof(ConsentController)
                .GetMethod(nameof(ConsentController.DismissCookieBanner))
                .GetCustomAttribute<AllowAnonymousAttribute>()
                .Should()
                .NotBeNull();
        }

        [Test]
        public static void DismissCookieBanner_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(ConsentController)
                .GetMethod(nameof(ConsentController.DismissCookieBanner))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should()
                .Be("/dismiss-cookie-banner");
        }

        [Test]
        public static void DismissCookieBanner_Sets_ExpectedCookie()
        {
            var expected = $"/organisation/09D/order/{Guid.NewGuid()}";
            using var controller = new ConsentController(new Mock<IAgreementConsentService>().Object, new Settings.CookieExpirationSettings())
            {
                ControllerContext = ControllerContext(Mock.Of<IResponseCookies>(), expected),
            };

            var actual = controller.DismissCookieBanner().As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(expected);
        }

        [Test]
        public static void DismissCookieBanner_Sets_RedirectsToReferer()
        {
            const int hours = 50;
            var settings = new Settings.CookieExpirationSettings
            {
                ConsentExpiration = TimeSpan.FromHours(hours),
            };
            var responseCookies = new Mock<IResponseCookies>();
            using var controller = new ConsentController(new Mock<IAgreementConsentService>().Object, settings)
            {
                ControllerContext = ControllerContext(responseCookies.Object, "/account"),
            };

            controller.DismissCookieBanner();

            responseCookies.Verify(
                r => r.Append(
                    Cookies.BuyingCatalogueConsent,
                    "true",
                    It.Is<CookieOptions>(
                        c => c.Expires.GetValueOrDefault() > DateTime.Now.AddHours(hours).AddMinutes(-2)
                            && c.Expires.GetValueOrDefault() < DateTime.Now.AddHours(hours))));
        }

        private static ControllerContext ControllerContext(string subjectId)
        {
            var claims = new[] { new Claim(JwtClaimTypes.Subject, subjectId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = Mock.Of<HttpContext>(c => c.User == principal),
            };
        }

        private static ControllerContext ControllerContext(IResponseCookies responseCookies, string referer)
        {
            var headerDictionary = new HeaderDictionary { { HeaderNames.Referer, new StringValues(referer) } };
            var request = Mock.Of<HttpRequest>(r => r.Headers == headerDictionary);
            var response = Mock.Of<HttpResponse>(
                r => r.Cookies == responseCookies);
            return new ControllerContext
            {
                HttpContext = Mock.Of<HttpContext>(c => c.Request == request && c.Response == response),
            };
        }
    }
}
