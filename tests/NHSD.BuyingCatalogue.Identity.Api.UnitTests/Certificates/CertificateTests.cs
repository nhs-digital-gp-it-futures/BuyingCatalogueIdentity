using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NUnit.Framework;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Certificates
{
    [TestFixture]
    internal sealed class CertificateTests
    {
        private const string CertificateName = ".certificate.pfx";

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_BadCertificatePath_ThrowsException()
        {
            var settings = new CertificateSettings
            {
                CertificatePath = "BadPath",
                CertificatePassword = "NHSD"
            };

            Assert.Throws<CertificateSettingsException>(() => new Certificate(settings, Mock.Of<ILogger>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_NullCertificate_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Certificate(null, Mock.Of<ILogger>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Constructor_NullLogger_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Certificate(new CertificateSettings(), null));
        }

        [Test]
        public void Constructor_ValidCertificatePath_InitializesAsExpected()
        {
            WriteEmbeddedCertificateToDisk();

            var settings = new CertificateSettings
            {
                CertificatePath = CertificateName,
                CertificatePassword = "NHSD"
            };

            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.Instance.Should().NotBeNull();
            certificate.Instance.Thumbprint.Should().Be("FD1F4371008CF3CACB6DE23D4AB2EC9623557DD4");
            certificate.IsAvailable.Should().BeTrue();
            certificate.Path.Should().Be(CertificateName);

            DeleteCertificateFile();
        }

        [Test]
        public void Instance_NullCertificate_ReturnsNull()
        {
            var settings = new CertificateSettings { UseDeveloperCredentials = true };
            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.Instance.Should().BeNull();
        }

        [Test]
        public void IsAvailable_NullCertificate_ReturnsFalse()
        {
            var settings = new CertificateSettings { UseDeveloperCredentials = true };
            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.IsAvailable.Should().BeFalse();
        }

        [Test]
        public void Path_NullCertificate_ReturnsNull()
        {
            var settings = new CertificateSettings { UseDeveloperCredentials = true };
            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.Path.Should().BeNull();
        }

        private static void DeleteCertificateFile()
        {
            File.Delete(CertificateName);
        }

        private static void WriteEmbeddedCertificateToDisk()
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(typeof(CertificateTests).Namespace + CertificateName);

            Assert.NotNull(stream, $"Could not find the embedded resource {CertificateName}");

            using var outputStream = File.Create(CertificateName);
            stream.CopyTo(outputStream);
        }
    }
}
