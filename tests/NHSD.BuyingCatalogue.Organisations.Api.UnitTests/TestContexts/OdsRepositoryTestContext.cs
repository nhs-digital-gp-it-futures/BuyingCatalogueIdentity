using LazyCache;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts
{
    internal sealed class OdsRepositoryTestContext
    {
        private OdsRepositoryTestContext()
        {
            OdsSettings = new OdsSettings
            {
                ApiBaseUrl = "https://fakeodsserver.net/ORD/2-0-0",
                BuyerOrganisationRoleIds = new[] { "RO98", "RO177", "RO213", "RO272" },
            };

            CachingService.DefaultCacheProvider.Value.Remove("XYZ");

            OdsRepository = new OdsRepository(OdsSettings, new CachingService());
        }

        public OdsRepository OdsRepository { get; set; }

        public OdsSettings OdsSettings { get; set; }

        internal static OdsRepositoryTestContext Setup()
        {
            return new();
        }
    }
}
