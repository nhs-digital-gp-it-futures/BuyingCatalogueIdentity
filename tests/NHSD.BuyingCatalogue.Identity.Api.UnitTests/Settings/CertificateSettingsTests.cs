using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Settings
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CertificateSettingsTests
    {
        [Test]
        public void ToStringIncludesCorrectFields()
        {
            var certificateSettings = new CertificateSettings
            {
                UseDeveloperCredentials = true, CertificatePath = "/Path", CertificatePassword = "Password"
            };
            var stringResult = certificateSettings.ToString();

            stringResult.Should().ContainAll("/Path", "True", "Pa******");
        }
    }
}
