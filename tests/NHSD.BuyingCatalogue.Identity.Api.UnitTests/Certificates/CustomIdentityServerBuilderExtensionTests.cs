using System.IO;
using System.Reflection;
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
    public class CustomIdentityServerBuilderExtensionTests
    {
        [Test]
        public void UseDeveloperCredentials_Sets_DeveloperCredentials()
        {
            var settings = new CertificateSettings
            {
                UseDeveloperCredentials = true,
                CertificatePath = "",
                CertificatePassword = ""
            };

            var loggerMock = Mock.Of<ILogger>();
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var sutMock = Mock.Of<IIdentityServerBuilder>(i => i.Services == serviceCollectionMock.Object);

            sutMock.AddCustomSigningCredential(settings, loggerMock);

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
                UseDeveloperCredentials = false,
                CertificatePath = "bad path",
                CertificatePassword = "NHSD"
            };

            var loggerMock = Mock.Of<ILogger>();
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var sutMock = Mock.Of<IIdentityServerBuilder>(i => i.Services == serviceCollectionMock.Object);
            Assert.Throws<CertificateSettingsException>(() => sutMock.AddCustomSigningCredential(settings, loggerMock));
        }

        [Test]
        public void UseValidPath_Sets_Certificate()
        {
            //write certificate file from embedded resource to disk
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(this.GetType().Namespace + ".certificate.pfx").ThrowIfNull();
            using (var outputStream = File.Create("certificate.pfx"))
            {
                stream.CopyTo(outputStream);
                stream.Flush();
                stream.Close();
            }

            var settings = new CertificateSettings
            {
                UseDeveloperCredentials = false, CertificatePath = "certificate.pfx", CertificatePassword = "NHSD"
            };

            var loggerMock = Mock.Of<ILogger>();
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var sutMock = Mock.Of<IIdentityServerBuilder>(i => i.Services == serviceCollectionMock.Object);

            sutMock.AddCustomSigningCredential(settings, loggerMock);

            serviceCollectionMock.Verify(
                c => c.Add(It.Is<ServiceDescriptor>(s => s.ServiceType == typeof(ISigningCredentialStore))),
                Times.Exactly(1));
            serviceCollectionMock.Verify(
                c => c.Add(It.Is<ServiceDescriptor>(s => s.ServiceType == typeof(IValidationKeysStore))),
                Times.Exactly(1));
            File.Delete("certificate.pfx");
        }
    }
}
