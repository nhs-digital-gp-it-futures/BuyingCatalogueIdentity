using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Certificates
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CertificateSettingsExceptionTests
    {
        [Test]
        public static void Constructor_Parameterless_InitializesCorrectly()
        {
            var certificateSettingsException = new CertificateSettingsException();

            certificateSettingsException.InnerException.Should().BeNull();
            certificateSettingsException.Message.Should().Be(CertificateSettingsException.DefaultMessage);
        }

        [Test]
        public static void Constructor_String_Exception_InitializesCorrectly()
        {
            const string message = "This is a message.";
            var innerException = new InvalidOperationException();

            var repositoryException = new CertificateSettingsException(message, innerException);

            repositoryException.InnerException.Should().Be(innerException);
            repositoryException.Message.Should().Be(message);
        }

        [Test]
        public static void Constructor_String_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var repositoryException = new CertificateSettingsException(message);

            repositoryException.InnerException.Should().BeNull();
            repositoryException.Message.Should().Be(message);
        }
    }
}
