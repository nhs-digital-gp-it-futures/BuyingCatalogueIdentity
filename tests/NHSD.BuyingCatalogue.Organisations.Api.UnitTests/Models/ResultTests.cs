using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class ResultTests
    {
        [Test]
        public void SuccessResult_IsSuccessIsTrue()
        {
            var actual = Result.Success();

            actual.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void FailureResult_EmptyErrors_IsSuccessIsFalse()
        {
            var actual = Result.Failure(Array.Empty<Error>());
            actual.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void FailureResult_NullErrorList_Errors_ReturnsEmptyList()
        {
            var actual = Result.Failure(null);

            actual.Errors.Should().BeEmpty();
        }

        [Test]
        public void FailureResult_OneError_Errors_ReturnsError()
        {
            var expectedErrors = new List<Error> { new Error("Test") };

            var actual = Result.Failure(expectedErrors);

            actual.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Test]
        public void TwoDifferenceResults_AreNotEqual()
        {
            var success = Result.Success();
            var failure = Result.Failure(Array.Empty<Error>());

            var actual = success.Equals(failure);

            actual.Should().BeFalse();
        }
    }
}
