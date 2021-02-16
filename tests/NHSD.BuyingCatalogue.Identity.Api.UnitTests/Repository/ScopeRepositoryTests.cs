using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Repository
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ScopeRepositoryTests
    {
        [Test]
        public static void Scopes_ApiAndIdentityResources_ReturnsExpectedScopes()
        {
            const string scope1 = "Scope1";
            const string scope2 = "Scope2";

            var expectedScopes = new[] { scope1, "scope2" };

            var apiResources = new[] { new ResourceSetting { ResourceName = scope1 } };
            var identityResources = new[] { new IdentityResourceSetting { ResourceType = scope2 } };

            var scopeRepository = new ScopeRepository(apiResources, identityResources);

            var actualScopes = scopeRepository.Scopes;

            actualScopes.Should().BeEquivalentTo(expectedScopes);
        }

        [Test]
        public static void Scopes_ApiResources_ReturnsExpectedScopes()
        {
            const string apiScope = "ApiScope1";

            var expectedScopes = new[] { apiScope };

            var apiResources = new[] { new ResourceSetting { ResourceName = apiScope } };

            var scopeRepository = new ScopeRepository(apiResources, null);

            var actualScopes = scopeRepository.Scopes;

            actualScopes.Should().BeEquivalentTo(expectedScopes);
        }

        [Test]
        public static void Scopes_IdentityResources_ReturnsExpectedScopes()
        {
            const string identityScope = "IdentityScope1";

            // ReSharper disable once StringLiteralTypo
            var expectedScopes = new[] { "identityscope1" };

            var identityResources = new[] { new IdentityResourceSetting { ResourceType = identityScope } };

            var scopeRepository = new ScopeRepository(null, identityResources);

            var actualScopes = scopeRepository.Scopes;

            actualScopes.Should().BeEquivalentTo(expectedScopes);
        }
    }
}
