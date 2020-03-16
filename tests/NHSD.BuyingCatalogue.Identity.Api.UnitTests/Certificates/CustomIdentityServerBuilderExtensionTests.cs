using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NUnit.Framework;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Certificates
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CustomIdentityServerBuilderExtensionTests
    {
        [Test]
        public void UseDeveloperCredentials_Sets_DeveloperCredentials()
        {
            var settings = new CertificateSettings
            {
                UseDeveloperCredentials = true, CertificatePath = "", CertificatePassword = ""
            };

            var loggerMock = Mock.Of<ILogger>();
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var builderMock = Mock.Of<IIdentityServerBuilder>(i => i.Services == serviceCollectionMock.Object);

            builderMock.AddCustomSigningCredential(settings, loggerMock);

            serviceCollectionMock.Verify(
                c => c.Add(It.Is<ServiceDescriptor>(s => s.ServiceType == typeof(ISigningCredentialStore))),
                Times.Exactly(1));
            serviceCollectionMock.Verify(
                c => c.Add(It.Is<ServiceDescriptor>(s => s.ServiceType == typeof(IValidationKeysStore))),
                Times.Exactly(1));
        }

        [Test]
        public void UseInvalidPath_ThrowsCertificateSettingsException()
        {
            var settings = new CertificateSettings
            {
                UseDeveloperCredentials = false, CertificatePath = "bad path", CertificatePassword = "NHSD"
            };

            var loggerMock = Mock.Of<ILogger>();
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var builderMock = Mock.Of<IIdentityServerBuilder>(i => i.Services == serviceCollectionMock.Object);
            Assert.Throws<CertificateSettingsException>(() =>
                builderMock.AddCustomSigningCredential(settings, loggerMock));
        }

        [Test]
        public async Task UseValidPath_Sets_Certificate()
        {
            SetupEmbeddedCertificateFile();

            var settings = new CertificateSettings
            {
                UseDeveloperCredentials = false, CertificatePath = "certificate.pfx", CertificatePassword = "NHSD"
            };

            var serviceDescriptorList = new List<ServiceDescriptor>();
            var loggerMock = Mock.Of<ILogger>();
            var serviceCollectionMock = new Mock<IServiceCollection>();
            serviceCollectionMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptorList.Add(sd));
            var builderMock = Mock.Of<IIdentityServerBuilder>(i => i.Services == serviceCollectionMock.Object);

            builderMock.AddCustomSigningCredential(settings, loggerMock);

            serviceCollectionMock.Verify(
                c => c.Add(It.Is<ServiceDescriptor>(s => s.ServiceType == typeof(ISigningCredentialStore))),
                Times.Exactly(1));
            serviceCollectionMock.Verify(
                c => c.Add(It.Is<ServiceDescriptor>(s => s.ServiceType == typeof(IValidationKeysStore))),
                Times.Exactly(1));

            if (serviceDescriptorList.First(x => x.ServiceType == typeof(ISigningCredentialStore)).ImplementationInstance is ISigningCredentialStore signingCredentialStore)
            {
                var signingCredential = await signingCredentialStore.GetSigningCredentialsAsync();
                signingCredential.Key.KeyId.Should().Be("FD1F4371008CF3CACB6DE23D4AB2EC9623557DD4");
            }

            RemoveCertificateFile();
        }

        private void SetupEmbeddedCertificateFile()
        {
            //write certificate file from embedded resource to disk, so it can be picked up by the file io operations
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(GetType().Namespace + ".certificate.pfx").ThrowIfNull();
            using var outputStream = File.Create("certificate.pfx");
            stream.CopyTo(outputStream);
        }

        private void RemoveCertificateFile()
        {
            File.Delete("certificate.pfx");
        }
    }
}
