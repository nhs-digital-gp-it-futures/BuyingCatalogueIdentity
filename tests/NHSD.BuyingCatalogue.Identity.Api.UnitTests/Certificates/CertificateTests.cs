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
    [Parallelizable(ParallelScope.All)]
    internal static class CertificateTests
    {
        private const string CertificateName = ".cert.crt";
        private const string CertificateKeyName = ".cert.key";

        private static readonly string[] CertificateFileNames = { CertificateName, CertificateKeyName };

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_BadCertificatePath_ThrowsException()
        {
            var settings = new CertificateSettings
            {
                CertificatePath = "BadPath",
                PrivateKeyPath = "NHSD",
            };

            Assert.Throws<CertificateSettingsException>(() => new Certificate(settings, Mock.Of<ILogger>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullCertificate_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Certificate(null, Mock.Of<ILogger>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_NullLogger_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Certificate(new CertificateSettings(), null));
        }

        [Test]
        public static void Constructor_ValidCertificatePath_InitializesAsExpected()
        {
            WriteEmbeddedFilesToDisk();

            var settings = new CertificateSettings
            {
                CertificatePath = CertificateName,
                PrivateKeyPath = CertificateKeyName,
            };

            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.Instance.Should().NotBeNull();
            certificate.Instance.Thumbprint.Should().Be("13C9ED6D78CBE4905367D5A99A66BF84B9D2E55F");
            certificate.IsAvailable.Should().BeTrue();
            certificate.Path.Should().Be(CertificateName);
            DeleteEmbeddedFiles();
        }

        [Test]
        public static void Instance_NullCertificate_ReturnsNull()
        {
            var settings = new CertificateSettings { UseDeveloperCredentials = true };
            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.Instance.Should().BeNull();
        }

        [Test]
        public static void IsAvailable_NullCertificate_ReturnsFalse()
        {
            var settings = new CertificateSettings { UseDeveloperCredentials = true };
            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.IsAvailable.Should().BeFalse();
        }

        [Test]
        public static void Path_NullCertificate_ReturnsNull()
        {
            var settings = new CertificateSettings { UseDeveloperCredentials = true };
            var certificate = new Certificate(settings, Mock.Of<ILogger>());

            certificate.Path.Should().BeNull();
        }

        private static void DeleteEmbeddedFiles()
        {
            foreach (var file in CertificateFileNames)
            {
                File.Delete(file);
            }
        }

        private static void WriteEmbeddedFilesToDisk()
        {
            foreach (var file in CertificateFileNames)
            {
                WriteEmbeddedFileToDisk(file);
            }
        }

        private static void WriteEmbeddedFileToDisk(string fileName)
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(typeof(CertificateTests).Namespace + fileName);

            Assert.NotNull(stream, $"Could not find the embedded resource {fileName}");

            using var outputStream = File.Create(fileName);
            stream.CopyTo(outputStream);
        }
    }
}
