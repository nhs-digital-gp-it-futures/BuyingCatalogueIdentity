﻿using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class ErrorTests
    {
        [Test]
        public void Constructor_String_InitializesCorrectly()
        {
            const string expectedId = "ErrorMessageId";

            var actual = new Error(expectedId);

            actual.Field.Should().BeNull();
            actual.Id.Should().Be(expectedId);
        }

        [Test]
        public void Constructor_String_String_InitializesCorrectly()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var actual = new Error(expectedId, expectedField);

            actual.Id.Should().Be(expectedId);
            actual.Field.Should().Be(expectedField);
        }

        [Test]
        public void Constructor_NullId_ThrowsException()
        {
            static void CreateError()
            {
                _ = new Error(null);
            }

            Assert.Throws<ArgumentNullException>(CreateError);
        }

        [Test]
        public void TwoErrorsWithSameProperties_AreEqual()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var first = new Error(expectedId, expectedField);
            var second = new Error(expectedId, expectedField);

            var actual = first.Equals(second);

            actual.Should().BeTrue();
        }

        [Test]
        public void TwoErrorsDifferentFieldValues_AreNotEqual()
        {
            const string expectedId = "ErrorMessageId";
            const string expectedField = "ErrorMessageId";

            var first = new Error(expectedId, expectedField);
            var second = new Error(expectedId, null);

            var actual = first.Equals(second);

            actual.Should().BeFalse();
        }
    }
}