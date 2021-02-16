using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AgreementConsentServiceTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullEventService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AgreementConsentService(null, Mock.Of<IIdentityServerInteractionService>(), Mock.Of<IScopeRepository>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullInteractionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AgreementConsentService(Mock.Of<IEventService>(), null, Mock.Of<IScopeRepository>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullScopeRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AgreementConsentService(Mock.Of<IEventService>(), Mock.Of<IIdentityServerInteractionService>(), null));
        }

        [Test]
        public static void IsValidReturnUrl_NullReturnUrl_ThrowsException()
        {
            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                Mock.Of<IIdentityServerInteractionService>(),
                Mock.Of<IScopeRepository>());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.IsValidReturnUrl(null));
        }

        [Test]
        public static async Task IsValidReturnUrl_BadReturnUrl_ReturnsFalse()
        {
            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                Mock.Of<IIdentityServerInteractionService>(),
                Mock.Of<IScopeRepository>());

            var isValidReturnUrl = await service.IsValidReturnUrl(new Uri("https://www.badurl.co.uk/"));

            isValidReturnUrl.Should().BeFalse();
        }

        [Test]
        public static async Task IsValidReturnUrl_ValidReturnUrl_ReturnsFalse()
        {
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService.Setup(i => i.GetAuthorizationContextAsync(It.IsNotNull<string>()))
                .ReturnsAsync(new AuthorizationRequest());

            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                mockInteractionService.Object,
                Mock.Of<IScopeRepository>());

            var isValidReturnUrl = await service.IsValidReturnUrl(new Uri("https://www.goodurl.co.uk/"));

            isValidReturnUrl.Should().BeTrue();
        }

        [Test]
        public static async Task GrantConsent_BadReturnUrl_ReturnsFailure()
        {
            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                Mock.Of<IIdentityServerInteractionService>(),
                Mock.Of<IScopeRepository>());

            var result = await service.GrantConsent(new Uri("https://www.badurl.co.uk/"), null);

            result.Should().Be(Result.Failure<Uri>());
        }

        [Test]
        public static async Task GrantConsent_ValidReturnUrl_GrantsExpectedConsent()
        {
            AuthorizationRequest actualContext = null;
            ConsentResponse actualConsentResponse = null;

            void GrantConsentCallback(AuthorizationRequest context, ConsentResponse response, string subject)
            {
                actualContext = context;
                actualConsentResponse = response;
            }

            var expectedContext = new AuthorizationRequest();
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService.Setup(i => i.GetAuthorizationContextAsync(It.IsNotNull<string>()))
                .ReturnsAsync(expectedContext);

            Expression<Func<IIdentityServerInteractionService, Task>> grantConsent = i => i.GrantConsentAsync(
                It.IsNotNull<AuthorizationRequest>(),
                It.IsNotNull<ConsentResponse>(),
                It.IsAny<string>());

            mockInteractionService.Setup(grantConsent)
                .Callback<AuthorizationRequest, ConsentResponse, string>(GrantConsentCallback);

            var expectedScopes = new[] { "Scope1", "Scope2" };
            var mockScopeRepository = Mock.Of<IScopeRepository>(r => r.Scopes == expectedScopes);

            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                mockInteractionService.Object,
                mockScopeRepository);

            await service.GrantConsent(new Uri("https://www.goodurl.co.uk/"), null);

            mockInteractionService.Verify(grantConsent);

            actualContext.Should().Be(expectedContext);
            actualConsentResponse.Should().BeEquivalentTo(new ConsentResponse { RememberConsent = true, ScopesConsented = expectedScopes });
        }

        [Test]
        public static async Task GrantConsent_ValidReturnUrl_RaisesConsentGrantedEvent()
        {
            const string subjectId = "SubjectId";
            const string clientId = "ClientId";

            var requestedScopes = new[] { "Scope1" };

            var context = new AuthorizationRequest { ClientId = clientId, ScopesRequested = requestedScopes };
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService.Setup(i => i.GetAuthorizationContextAsync(It.IsNotNull<string>()))
                .ReturnsAsync(context);

            var expectedScopes = new[] { "Scope1", "Scope2" };
            var mockScopeRepository = Mock.Of<IScopeRepository>(r => r.Scopes == expectedScopes);

            Expression<Action<IEventService>> raise = e => e.RaiseAsync(It.IsNotNull<Event>());
            Event actualEvent = null;

            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(raise)
                .Callback<Event>(e => actualEvent = e);

            var service = new AgreementConsentService(
                mockEventService.Object,
                mockInteractionService.Object,
                mockScopeRepository);

            await service.GrantConsent(new Uri("https://www.goodurl.co.uk/"), subjectId);

            mockEventService.Verify(raise);

            actualEvent.Should().BeOfType<ConsentGrantedEvent>();
            actualEvent.Should().BeEquivalentTo(new ConsentGrantedEvent(
                subjectId,
                clientId,
                requestedScopes,
                expectedScopes,
                true));
        }

        [Test]
        public static async Task GrantConsent_ValidReturnUrl_ReturnsSuccess()
        {
            var mockInteractionService = new Mock<IIdentityServerInteractionService>();
            mockInteractionService.Setup(i => i.GetAuthorizationContextAsync(It.IsNotNull<string>()))
                .ReturnsAsync(new AuthorizationRequest());

            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                mockInteractionService.Object,
                Mock.Of<IScopeRepository>());

            var returnUrl = new Uri("https://www.goodurl.co.uk/");
            var result = await service.GrantConsent(returnUrl, null);

            result.Should().Be(Result.Success(returnUrl));
        }

        [Test]
        public static void GrantConsent_NullReturnUrl_ThrowsException()
        {
            var service = new AgreementConsentService(
                Mock.Of<IEventService>(),
                Mock.Of<IIdentityServerInteractionService>(),
                Mock.Of<IScopeRepository>());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GrantConsent(null, string.Empty));
        }
    }
}
