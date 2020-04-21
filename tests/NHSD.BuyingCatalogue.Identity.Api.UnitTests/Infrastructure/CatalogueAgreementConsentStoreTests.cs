using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Infrastructure
{
    [TestFixture]
    internal sealed class CatalogueAgreementConsentStoreTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_IScopeRepository_IUsersRepository_NullScopeRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CatalogueAgreementConsentStore(null, Mock.Of<IUsersRepository>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_IScopeRepository_IUsersRepository_NullUsersRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CatalogueAgreementConsentStore(Mock.Of<IScopeRepository>(), null));
        }

        [Test]
        public void StoreUserConsentAsync_NullConsent_ThrowsException()
        {
            var store = new CatalogueAgreementConsentStore(Mock.Of<IScopeRepository>(), Mock.Of<IUsersRepository>());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await store.StoreUserConsentAsync(null));
        }

        [Test]
        public async Task StoreUserConsentAsync_MarksCatalogueAgreementSigned()
        {
            const string subjectId = "JulesVerneId";

            var user = ApplicationUserBuilder.Create().Build();
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(
                u => u.GetByIdAsync(It.Is<string>(s => s.Equals(subjectId, StringComparison.Ordinal))))
                .ReturnsAsync(user);

            var store = new CatalogueAgreementConsentStore(Mock.Of<IScopeRepository>(), mockUsersRepository.Object);

            user.CatalogueAgreementSigned.Should().BeFalse();

            await store.StoreUserConsentAsync(new Consent { SubjectId = subjectId });

            user.CatalogueAgreementSigned.Should().BeTrue();
        }

        [Test]
        public async Task StoreUserConsentAsync_UpdatesRepository()
        {
            var user = ApplicationUserBuilder.Create().Build();
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(u => u.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var store = new CatalogueAgreementConsentStore(Mock.Of<IScopeRepository>(), mockUsersRepository.Object);

            await store.StoreUserConsentAsync(new Consent());

            mockUsersRepository.Verify(r => r.UpdateAsync(It.Is<ApplicationUser>(u => u == user)), Times.Once());
        }

        [Test]
        public async Task GetUserConsentAsync_CatalogueAgreementNotSigned_ReturnsConsentWithNoScopes()
        {
            const string clientId = "SomeClient";
            const string subjectId = "JulesVerneId";

            var expectedConsent = new Consent
            {
                ClientId = clientId,
                Expiration = DateTime.MaxValue,
                Scopes = null,
                SubjectId = subjectId,
            };

            var user = ApplicationUserBuilder.Create().Build();
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(
                u => u.GetByIdAsync(It.Is<string>(s => s.Equals(subjectId, StringComparison.Ordinal))))
                .ReturnsAsync(user);

            var store = new CatalogueAgreementConsentStore(Mock.Of<IScopeRepository>(), mockUsersRepository.Object);

            var actualConsent = await store.GetUserConsentAsync(subjectId, clientId);

            actualConsent.Should().BeEquivalentTo(expectedConsent, o => o.Excluding(c => c.CreationTime));
        }

        [Test]
        public async Task GetUserConsentAsync_CatalogueAgreementSigned_ReturnsConsentWithScopes()
        {
            const string clientId = "SomeClient";
            const string subjectId = "JulesVerneId";

            var scopes = new[] { "Scope1", "Scope2" };

            var expectedConsent = new Consent
            {
                ClientId = clientId,
                Expiration = DateTime.MaxValue,
                Scopes = scopes,
                SubjectId = subjectId,
            };

            var mockScopeRepository = new Mock<IScopeRepository>();
            mockScopeRepository.Setup(s => s.Scopes).Returns(scopes);

            var user = ApplicationUserBuilder
                .Create()
                .WithCatalogueAgreementSigned(true)
                .Build();

            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(
                    u => u.GetByIdAsync(It.Is<string>(s => s.Equals(subjectId, StringComparison.Ordinal))))
                .ReturnsAsync(user);

            var store = new CatalogueAgreementConsentStore(mockScopeRepository.Object, mockUsersRepository.Object);

            var actualConsent = await store.GetUserConsentAsync(subjectId, clientId);

            actualConsent.Should().BeEquivalentTo(expectedConsent, o => o.Excluding(c => c.CreationTime));
        }

        [Test]
        public void RemoveUserConsentAsync_ReturnsCompletedTask()
        {
            var store = new CatalogueAgreementConsentStore(Mock.Of<IScopeRepository>(), Mock.Of<IUsersRepository>());

            var response = store.RemoveUserConsentAsync(null, null);

            response.Should().Be(Task.CompletedTask);
        }
    }
}
