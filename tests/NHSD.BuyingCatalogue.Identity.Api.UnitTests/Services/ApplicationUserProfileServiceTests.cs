using System.Threading.Tasks;
using IdentityServer4.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    public sealed class ApplicationUserProfileServiceTests
    {
        [Test]
        public async Task GetProfileDataAsync_()
        {
            var sut = new ApplicationUserProfileService(null, null);

            await sut.GetProfileDataAsync(new ProfileDataRequestContext("SubjectId", null, null, null));
        }
    }
}
