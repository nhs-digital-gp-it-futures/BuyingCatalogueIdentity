using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Consent;
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
            Assert.Throws<ArgumentNullException>(() => _ = new ConsentController(null));
        }

        [Test]
        public static void Index_Uri_NullReturnUrl_ThrowsException()
        {
            static async Task Index()
            {
                using var controller = new ConsentController(Mock.Of<IAgreementConsentService>());
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

            using var controller = new ConsentController(mockConsentService.Object);

            var result = await controller.Index(returnUrl) as ViewResult;

            Assert.NotNull(result);
            result.Model.Should().BeEquivalentTo(new ConsentViewModel { ReturnUrl = returnUrl });
        }

        [Test]
        public static async Task Index_Uri_BadReturnUrl_ReturnsErrorView()
        {
            using var controller = new ConsentController(Mock.Of<IAgreementConsentService>());

            var result = await controller.Index(new Uri("http://www.badurl.co.uk/")) as ViewResult;

            Assert.NotNull(result);
            result.ViewName.Should().Be("Error");
        }

        [Test]
        public static void Index_ConsentViewModel_NullModel_ThrowsException()
        {
            static async Task Index()
            {
                using var controller = new ConsentController(Mock.Of<IAgreementConsentService>());
                await controller.Index((ConsentViewModel)null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(Index);
        }

        [Test]
        public static async Task Index_ConsentViewModel_InvalidModelState_ReturnsConsentView()
        {
            var expectedModel = new ConsentViewModel();

            using var controller = new ConsentController(Mock.Of<IAgreementConsentService>());
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

            using var controller = new ConsentController(mockConsentService.Object)
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

            using var controller = new ConsentController(mockConsentService.Object)
            {
                ControllerContext = ControllerContext(string.Empty),
            };

            var result = await controller.Index(new ConsentViewModel()) as ViewResult;

            Assert.NotNull(result);
            result.ViewName.Should().Be("Error");
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
    }
}
