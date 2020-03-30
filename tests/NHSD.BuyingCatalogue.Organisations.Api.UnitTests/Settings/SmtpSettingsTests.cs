using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Settings
{
    [TestFixture]
    internal sealed class SmtpSettingsTests
    {
        [Test]
        public void SmtpSettings_AuthenticationSettings_IsInitialized()
        {
            var settings = new SmtpSettings();

            settings.Authentication.Should().NotBeNull();
        }
    }
}
