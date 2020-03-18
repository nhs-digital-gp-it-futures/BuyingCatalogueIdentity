using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Certificates;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Certificates
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class CertificateSettingsExceptionTests
    {
        [Test]
        public void Constructor_Parameterless_InitializesCorrectly()
        {
            var certificateSettingsException = new CertificateSettingsException();
            
            certificateSettingsException.InnerException.Should().BeNull();
            certificateSettingsException.Message.Should().Be(CertificateSettingsException.DefaultMessage);
        }

        [Test]
        public void Constructor_String_Exception_InitializesCorrectly()
        {
            const string message = "This is a message.";
            var innerException = new InvalidOperationException();

            var repoException = new CertificateSettingsException(message, innerException);
            
            repoException.InnerException.Should().Be(innerException);
            repoException.Message.Should().Be(message);
        }

        [Test]
        public void Constructor_String_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var repoException = new CertificateSettingsException(message);

            repoException.InnerException.Should().BeNull();
            repoException.Message.Should().Be(message);
        }
    }
}
