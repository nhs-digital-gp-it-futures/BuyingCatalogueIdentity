using System;
using NHSD.BuyingCatalogue.Identity.Api.Exceptions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class InvalidReturnUrlExceptionTests
    {
        [Test]
        public void Constructor_Parameterless_InitializesCorrectly()
        {
            var urlException = new InvalidReturnUrlException();

            Assert.Null(urlException.InnerException);
            Assert.AreEqual(InvalidReturnUrlException.DefaultMessage, urlException.Message);
        }

        [Test]
        public void Constructor_String_Exception_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var innerException = new InvalidOperationException();
            var urlException = new InvalidReturnUrlException(message, innerException);

            Assert.AreEqual(innerException, urlException.InnerException);
            Assert.AreEqual(message, urlException.Message);
        }

        [Test]
        public void Constructor_String_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var urlException = new InvalidReturnUrlException(message);

            Assert.Null(urlException.InnerException);
            Assert.AreEqual(message, urlException.Message);
        }
    }
}
