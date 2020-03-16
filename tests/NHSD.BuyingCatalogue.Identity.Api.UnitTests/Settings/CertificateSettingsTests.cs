using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Settings
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class CertificateSettingsTests
    {
        [Test]
        public void ToStringIncludesCorrectFields()
        {
            var certificateSettings = new CertificateSettings
            {
                UseDeveloperCredentials = true, CertificatePath = "/Path", CertificatePassword = "Password"
            };
            var stringResult = certificateSettings.ToString();

            stringResult.Should().ContainAll(new[] {"/Path", "True", "Pa******"});
        }
    }
}
