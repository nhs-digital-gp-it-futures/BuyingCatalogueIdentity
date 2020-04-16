using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts
{
    internal sealed class OdsRepositoryTestContext
    {
        public OdsRepository OdsRepository { get; set; }

        public OdsSettings OdsSettings { get; set; }

        private OdsRepositoryTestContext()
        {
            OdsSettings = new OdsSettings
            {
                ApiBaseUrl = "https://fakeodsserver.net", 
                BuyerOrganisationRoleIds = new[] {"RO98", "RO213"}
            };
            OdsRepository = new OdsRepository(OdsSettings);
        }

        internal static OdsRepositoryTestContext Setup()
        {
            return new OdsRepositoryTestContext();
        }
    }
}
