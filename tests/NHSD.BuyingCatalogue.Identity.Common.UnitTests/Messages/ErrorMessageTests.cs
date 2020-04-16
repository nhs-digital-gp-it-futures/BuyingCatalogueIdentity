using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Messages
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class ErrorMessageTests
    {
        [Test]
        public void Constructor_String_InitializesCorrectly()
        {
            const string expectedId = "ErrorMessageId";

            var actual = new ErrorDetails(expectedId);

            actual.Field.Should().BeNull();
            actual.Id.Should().Be(expectedId);
        }

        [Test]
        public void Constructor_String_String_InitializesCorrectly()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var actual = new ErrorDetails(expectedId, expectedField);

            actual.Id.Should().Be(expectedId);
            actual.Field.Should().Be(expectedField);
        }

        [Test]
        public void Constructor_NullId_ThrowsException()
        {
            static void CreateError()
            {
                _ = new ErrorDetails(null);
            }

            Assert.Throws<ArgumentNullException>(CreateError);
        }

        [Test]
        public void TwoErrorsWithSameProperties_AreEqual()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var first = new ErrorDetails(expectedId, expectedField);
            var second = new ErrorDetails(expectedId, expectedField);

            var actual = first.Equals(second);

            actual.Should().BeTrue();
        }

        [Test]
        public void TwoErrorsDifferentFieldValues_AreNotEqual()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var first = new ErrorDetails(expectedId, expectedField);
            var second = new ErrorDetails(expectedId, null);

            var actual = first.Equals(second);

            actual.Should().BeFalse();
        }
    }
}
