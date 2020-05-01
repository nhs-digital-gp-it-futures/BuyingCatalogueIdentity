using System;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.DependencyInjection;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.DependencyInjection
{
    [TestFixture]
    internal sealed class ServiceCollectionExtensionsTests
    {
        [Test]
        public void AddDataProtection_NullCertificate_ThrowsException()
        {
            var services = Mock.Of<IServiceCollection>();

            Assert.Throws<ArgumentNullException>(
                () => services.AddDataProtection(null, null));
        }

        [Test]
        public void AddDataProtection_NullServiceCollection_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => ServiceCollectionExtensions.AddDataProtection(null, null, Mock.Of<ICertificate>()));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public void AddDataProtection_NullEmptyWhiteSpaceAppName_DoesNothing(string applicationName)
        {
            var mockServices = new Mock<IServiceCollection>();
            var services = mockServices.Object;

            services.AddDataProtection(applicationName, Mock.Of<ICertificate>());

            mockServices.VerifyNoOtherCalls();
        }

        [Test]
        public void AddDataProtection_WithAppName_WithoutCert_AddsDataProtection()
        {
            var services = new ServiceCollection();

            services.AddDataProtection("AppName", Mock.Of<ICertificate>());

            using var provider = services.BuildServiceProvider();
            var dpProvider = provider.GetRequiredService<IDataProtectionProvider>();

            dpProvider.Should().NotBeNull();
        }

        [Test]
        public void AddDataProtection_WithAppName_WithCert_AddsDataProtection()
        {
            using var x509 = new X509Certificate2();

            var certificate = new Mock<ICertificate>();
            certificate.Setup(c => c.IsAvailable).Returns(true);
            certificate.Setup(c => c.Instance).Returns(x509);

            var services = new ServiceCollection();

            services.AddDataProtection("AppName", certificate.Object);

            using var provider = services.BuildServiceProvider();
            var dpProvider = provider.GetRequiredService<IDataProtectionProvider>();

            dpProvider.Should().NotBeNull();
        }

        [Test]
        public void AddDataProtection_WithAppName_WithoutCert_SetsAppDiscriminator()
        {
            var fixture = new Fixture();
            var appName = fixture.Create<string>();

            var services = new ServiceCollection();

            services.AddDataProtection(appName, Mock.Of<ICertificate>());

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<DataProtectionOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.ApplicationDiscriminator.Should().Be(appName);
        }

        [Test]
        public void AddDataProtection_WithAppName_WithCert_SetsAppDiscriminator()
        {
            var fixture = new Fixture();

            var appName = fixture.Create<string>();
            using var x509 = new X509Certificate2();

            var certificate = new Mock<ICertificate>();
            certificate.Setup(c => c.IsAvailable).Returns(true);
            certificate.Setup(c => c.Instance).Returns(x509);
            var services = new ServiceCollection();

            services.AddDataProtection(appName, certificate.Object);

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<DataProtectionOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.ApplicationDiscriminator.Should().Be(appName);
        }

        [Test]
        public void AddDataProtection_WithAppName_WithoutCert_SetsXmlRepository()
        {
            var services = new ServiceCollection();

            services.AddDataProtection("AppName", Mock.Of<ICertificate>());

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.XmlRepository.Should().NotBeNull();
            options.Value.XmlRepository.Should().BeOfType<EntityFrameworkCoreXmlRepository<ApplicationDbContext>>();
        }

        [Test]
        public void AddDataProtection_WithAppName_WithCert_SetsXmlRepository()
        {
            using var x509 = new X509Certificate2();

            var certificate = new Mock<ICertificate>();
            certificate.Setup(c => c.IsAvailable).Returns(true);
            certificate.Setup(c => c.Instance).Returns(x509);

            var services = new ServiceCollection();

            services.AddDataProtection("AppName", certificate.Object);

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.XmlRepository.Should().NotBeNull();
            options.Value.XmlRepository.Should().BeOfType<EntityFrameworkCoreXmlRepository<ApplicationDbContext>>();
        }

        [Test]
        public void AddDataProtection_WithAppName_WithoutCert_DoesNotSetXmlEncryptor()
        {
            var services = new ServiceCollection();

            services.AddDataProtection("AppName", Mock.Of<ICertificate>());

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.XmlEncryptor.Should().BeNull();
        }

        [Test]
        public void AddDataProtection_WithAppName_WithCert_SetsXmlEncryptor()
        {
            using var x509 = new X509Certificate2();

            var certificate = new Mock<ICertificate>();
            certificate.Setup(c => c.IsAvailable).Returns(true);
            certificate.Setup(c => c.Instance).Returns(x509);

            var services = new ServiceCollection();

            services.AddDataProtection("AppName", certificate.Object);

            using var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>();

            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();

            var xmlEncryptor = options.Value.XmlEncryptor;
            xmlEncryptor .Should().NotBeNull();
            xmlEncryptor .Should().BeOfType<CertificateXmlEncryptor>();
        }
    }
}
