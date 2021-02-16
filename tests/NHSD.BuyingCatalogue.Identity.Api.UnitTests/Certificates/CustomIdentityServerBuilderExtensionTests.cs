using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer4.Configuration;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NUnit.Framework;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Certificates
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    internal static class CustomIdentityServerBuilderExtensionTests
    {
        private const string DeveloperKeyFileName = "tempkey.rsa";

        [Test]
        public static void AddCustomSigningCredential_NullCertificate_ThrowsException()
        {
            var builder = Mock.Of<IIdentityServerBuilder>();

            Assert.Throws<ArgumentNullException>(
                () => builder.AddCustomSigningCredential(null, Mock.Of<ILogger>()));
        }

        [Test]
        public static void AddCustomSigningCredential_NullLogger_ThrowsException()
        {
            var builder = Mock.Of<IIdentityServerBuilder>();

            Assert.Throws<ArgumentNullException>(
                () => builder.AddCustomSigningCredential(Mock.Of<ICertificate>(), null));
        }

        [Test]
        public static async Task AddCustomSigningCredential_WithCertificate_SetsSigningCredentials()
        {
            using var x509 = CreateValidCertificate();

            var mockCertificate = new Mock<ICertificate>();
            mockCertificate.Setup(c => c.Instance).Returns(x509);
            mockCertificate.Setup(c => c.IsAvailable).Returns(true);

            var services = new ServiceCollection();
            var builder = new IdentityServerBuilder(services);

            builder.AddCustomSigningCredential(mockCertificate.Object, Mock.Of<ILogger>());

            var provider = services.BuildServiceProvider();
            var signingCredentialStore = provider.GetService<ISigningCredentialStore>();
            var validationKeysStore = provider.GetService<IValidationKeysStore>();

            Assert.NotNull(signingCredentialStore);
            Assert.NotNull(validationKeysStore);

            var signingCredentials = await signingCredentialStore.GetSigningCredentialsAsync();
            var validationKeys = (await validationKeysStore.GetValidationKeysAsync()).ToList();

            signingCredentials.Should().NotBeNull();
            signingCredentials.Kid.Should().Be(x509.Thumbprint);

            validationKeys.Should().NotBeNull();
            validationKeys.Should().HaveCount(1);

            var securityKey = validationKeys.First();
            securityKey.Key.KeyId.Should().Be(x509.Thumbprint);
        }

        [Test]
        public static async Task AddCustomSigningCredential_WithoutCertificate_SetsDeveloperCredentials()
        {
            // ReSharper disable once StringLiteralTypo
            const string expectedKeyId = "xEB-cwCvL4brqLxCMzkw3Q";

            WriteEmbeddedDeveloperCertificateToDisk();

            var services = new ServiceCollection();
            var builder = new IdentityServerBuilder(services);

            builder.AddCustomSigningCredential(Mock.Of<ICertificate>(), Mock.Of<ILogger>());

            var provider = services.BuildServiceProvider();
            var signingCredentialStore = provider.GetService<ISigningCredentialStore>();
            var validationKeysStore = provider.GetService<IValidationKeysStore>();

            Assert.NotNull(signingCredentialStore);
            Assert.NotNull(validationKeysStore);

            var signingCredentials = await signingCredentialStore.GetSigningCredentialsAsync();
            var validationKeys = (await validationKeysStore.GetValidationKeysAsync()).ToList();

            signingCredentials.Should().NotBeNull();
            signingCredentials.Kid.Should().Be(expectedKeyId);

            validationKeys.Should().NotBeNull();
            validationKeys.Should().HaveCount(1);

            var securityKey = validationKeys.First();
            securityKey.Key.KeyId.Should().Be(expectedKeyId);

            DeleteDeveloperCertificate();
        }

        private static X509Certificate2 CreateValidCertificate()
        {
            var fixture = new Fixture();
            var subject = fixture.Create<string>();

            // ReSharper disable once IdentifierTypo
            using var ecdsa = ECDsa.Create();
            var req = new CertificateRequest($"cn={subject}", ecdsa!, HashAlgorithmName.SHA256);

            return req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1));
        }

        private static void DeleteDeveloperCertificate()
        {
            File.Delete(DeveloperKeyFileName);
        }

        private static void WriteEmbeddedDeveloperCertificateToDisk()
        {
            // ReSharper disable once StringLiteralTypo
            const string embeddedDeveloperKeyFileName = "tempkey.key";

            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"{typeof(CustomIdentityServerBuilderExtensionTests).Namespace}.{embeddedDeveloperKeyFileName}");

            Assert.NotNull(stream, $"Could not find the embedded resource {embeddedDeveloperKeyFileName}");

            using var outputStream = File.Create(DeveloperKeyFileName);
            stream.CopyTo(outputStream);
        }
    }
}
