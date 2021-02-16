﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Results
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ResultTests
    {
        [Test]
        public static void SuccessResult_IsSuccessIsTrue()
        {
            var actual = Result.Success();

            actual.IsSuccess.Should().BeTrue();
        }

        [Test]
        public static void SuccessResultT_IsSuccessIsTrue()
        {
            var actual = Result.Success("Test");

            actual.IsSuccess.Should().BeTrue();
        }

        [Test]
        public static void SuccessResultT_ReturnsValue()
        {
            var actual = Result.Success("Test");

            actual.Value.Should().Be("Test");
        }

        [Test]
        public static void FailureResult_EmptyErrors_IsSuccessIsFalse()
        {
            var actual = Result.Failure(Array.Empty<ErrorDetails>());
            actual.IsSuccess.Should().BeFalse();
        }

        [Test]
        public static void FailureResultT_EmptyErrors_IsSuccessIsFalse()
        {
            var actual = Result.Failure<string>(Array.Empty<ErrorDetails>());
            actual.IsSuccess.Should().BeFalse();
        }

        [Test]
        public static void FailureResult_NullErrorList_Errors_ReturnsEmptyList()
        {
            var actual = Result.Failure(null!);

            actual.Errors.Should().BeEmpty();
        }

        [Test]
        public static void FailureResultT_NullErrorList_Errors_ReturnsEmptyList()
        {
            var actual = Result.Failure<string>(null!);

            actual.Errors.Should().BeEmpty();
        }

        [Test]
        public static void FailureResult_OneError_Errors_ReturnsError()
        {
            var expectedErrors = new List<ErrorDetails> { new("Test") };

            var actual = Result.Failure(expectedErrors);

            actual.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Test]
        public static void FailureResultT_OneError_Errors_ReturnsError()
        {
            var expectedErrors = new List<ErrorDetails> { new("Test") };

            var actual = Result.Failure<string>(expectedErrors);

            actual.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Test]
        public static void TwoDifferenceResults_AreNotEqual()
        {
            var success = Result.Success();
            var failure = Result.Failure(Array.Empty<ErrorDetails>());

            var actual = success.Equals(failure);

            actual.Should().BeFalse();
        }

        [Test]
        public static void TwoDifferentResultsT_AreNotEqual()
        {
            var success = Result.Success("TestA");
            var failure = Result.Success("TestB");

            var actual = success.Equals(failure);

            actual.Should().BeFalse();
        }

        [Test]
        public static void ToResult_ConvertSuccessResultT_ReturnsSuccessResult()
        {
            var sut = Result.Success("Test");

            var actual = sut.ToResult();

            actual.Should().Be(Result.Success());
        }

        [Test]
        public static void ToResult_ConvertFailureResultT_ReturnsFailureResult()
        {
            List<ErrorDetails> expectedErrors = new List<ErrorDetails> { new("TestErrorId") };
            var sut = Result.Failure<string>(expectedErrors);

            var actual = sut.ToResult();

            actual.Should().Be(Result.Failure(expectedErrors));
        }

        [Test]
        public static void FailureResultT_ReturnsDefaultValue()
        {
            var actual = Result.Failure<int>(Array.Empty<ErrorDetails>());
            actual.Value.Should().Be(default);
        }
    }
}
