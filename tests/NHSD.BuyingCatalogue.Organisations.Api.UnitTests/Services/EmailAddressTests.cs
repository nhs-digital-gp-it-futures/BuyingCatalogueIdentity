using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class EmailAddressTests
    {
        [Test]
        public void Constructor_String_String_InitializesExpectedValues()
        {
            const string name = "Some Body";
            const string address = "somebody@notarealaddress.co.uk";

            var emailAddress = new EmailAddress(name, address);

            emailAddress.DisplayName.Should().Be(name);
            emailAddress.Address.Should().Be(address);
        }

        [Test]
        [TestCase("")]
        [TestCase("\t")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Address_Set_EmptyOrWhiteSpaceAddress_ThrowsException(string address)
        {
            Assert.Throws<ArgumentException>(() => new EmailAddress { Address = address });
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public void Address_Set_NullAddress_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailAddress { Address = null });
        }
    }
}
