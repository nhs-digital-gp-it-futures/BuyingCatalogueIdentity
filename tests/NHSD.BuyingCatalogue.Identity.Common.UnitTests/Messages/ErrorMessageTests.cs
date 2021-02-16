using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Messages
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ErrorMessageTests
    {
        [Test]
        public static void Constructor_String_InitializesCorrectly()
        {
            const string expectedId = "ErrorMessageId";

            var actual = new ErrorDetails(expectedId);

            actual.Field.Should().BeNull();
            actual.Id.Should().Be(expectedId);
        }

        [Test]
        public static void Constructor_String_String_InitializesCorrectly()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var actual = new ErrorDetails(expectedId, expectedField);

            actual.Id.Should().Be(expectedId);
            actual.Field.Should().Be(expectedField);
        }

        [Test]
        public static void Constructor_NullId_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ErrorDetails(null!));
        }

        [Test]
        public static void TwoErrorsWithSameProperties_AreEqual()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var first = new ErrorDetails(expectedId, expectedField);
            var second = new ErrorDetails(expectedId, expectedField);

            var actual = first.Equals(second);

            actual.Should().BeTrue();
        }

        [Test]
        public static void TwoErrorsDifferentFieldValues_AreNotEqual()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var first = new ErrorDetails(expectedId, expectedField);
            var second = new ErrorDetails(expectedId);

            var actual = first.Equals(second);

            actual.Should().BeFalse();
        }
    }
}
