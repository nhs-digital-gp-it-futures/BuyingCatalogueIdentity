using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class PasswordResetCallbackTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_IHttpContextAccessor_LinkGenerator_NullAccessor_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new PasswordResetCallback(null, Mock.Of<LinkGenerator>(), new Api.Settings.IssuerSettings()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_IHttpContextAccessor_LinkGenerator_NullGenerator_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new PasswordResetCallback(Mock.Of<IHttpContextAccessor>(), null,new Api.Settings.IssuerSettings()));
        }

        [Test]
        public void GetPasswordResetCallback_NullToken_ThrowsException()
        {
            var callback = new PasswordResetCallback(Mock.Of<IHttpContextAccessor>(), Mock.Of<LinkGenerator>(), new Api.Settings.IssuerSettings());

            Assert.Throws<ArgumentNullException>(() => callback.GetPasswordResetCallback(null));
        }

        [Test]
        public void GetPasswordResetCallback_GetsExpectedAction()
        {
            const string expectedToken = "IAmBecomeToken";
            var expectedValues = new RouteValueDictionary
            {
                { "Email", "a.b@c.com" },
                { "Token", expectedToken },
                { "action", nameof(AccountController.ResetPassword) },
                { "controller", "Account" }
            };

            var context = new PasswordResetCallbackContext("https://www.google.co.uk/");
            var callback = context.Callback;

            callback.GetPasswordResetCallback(
                new PasswordResetToken(expectedToken, ApplicationUserBuilder.Create().Build()));

            context.RouteValues.Should().BeEquivalentTo(expectedValues);
        }

        [Test]
        public void GetPasswordResetCallback_ReturnsExpectedValue()
        {
            const string url = "https://nhs.uk/reset";

            var expectedUri = new Uri(url);

            var context = new PasswordResetCallbackContext(url);
            var callback = context.Callback;

            var actualUri = callback.GetPasswordResetCallback(
                new PasswordResetToken("Token", ApplicationUserBuilder.Create().Build()));

            actualUri.Should().Be(expectedUri);
        }

        private class PasswordResetCallbackContext
        {
            private readonly Mock<IHttpContextAccessor> _mockAccessor = new Mock<IHttpContextAccessor>();
            private readonly Mock<LinkGenerator> _mockGenerator = new Mock<LinkGenerator>();

            internal PasswordResetCallbackContext(string url)
            {
                var mockContext = new Mock<HttpContext>();
                mockContext.Setup(c => c.Request).Returns(Mock.Of<HttpRequest>());
                mockContext.Setup(c => c.Features).Returns(new FeatureCollection());

                _mockAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);
                _mockGenerator.Setup(g => g.GetUriByAddress(
                        It.IsAny<HttpContext>(),
                        It.IsAny<RouteValuesAddress>(),
                        It.IsAny<RouteValueDictionary>(),
                        It.IsAny<RouteValueDictionary>(),
                        It.IsAny<string>(),
                        It.IsAny<HostString?>(),
                        It.IsAny<PathString?>(),
                        It.IsAny<FragmentString>(),
                        It.IsAny<LinkOptions>()))
                    .Callback<
                        HttpContext,
                        RouteValuesAddress,
                        RouteValueDictionary,
                        RouteValueDictionary,
                        string,
                        HostString?,
                        PathString?,
                        FragmentString,
                        LinkOptions>(GetUriByAddressCallback)
                    .Returns(url);
            }

            internal PasswordResetCallback Callback => new PasswordResetCallback(
                _mockAccessor.Object, _mockGenerator.Object, new Api.Settings.IssuerSettings { IssuerUrl = new Uri("http://www.google.com") });

            internal RouteValueDictionary RouteValues { get; private set; }

            private void GetUriByAddressCallback(
                HttpContext httpContext,
                RouteValuesAddress address,
                RouteValueDictionary values,
                RouteValueDictionary ambientValues,
                string scheme,
                HostString? host,
                PathString? pathBase,
                FragmentString fragment,
                LinkOptions options)
            {
                RouteValues = values;
            }
        }
    }
}
