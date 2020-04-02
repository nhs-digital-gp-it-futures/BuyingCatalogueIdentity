using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Models.Results;
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
        public void SuccessResultT_IsSuccessIsTrue()
        {
            var actual = Result.Success("Test");

            actual.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void SuccessResultT_ReturnsValue()
        {
            var actual = Result.Success("Test");

            actual.Value.Should().Be("Test");
        }

        [Test]
        public void FailureResult_EmptyErrors_IsSuccessIsFalse()
        {
            var actual = Result.Failure(Array.Empty<ErrorMessage>());
            actual.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void FailureResultT_EmptyErrors_IsSuccessIsFalse()
        {
            var actual = Result.Failure<string>(Array.Empty<ErrorMessage>());
            actual.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void FailureResult_NullErrorList_Errors_ReturnsEmptyList()
        {
            var actual = Result.Failure(null);

            actual.Errors.Should().BeEmpty();
        }

        [Test]
        public void FailureResultT_NullErrorList_Errors_ReturnsEmptyList()
        {
            var actual = Result.Failure<string>(null);

            actual.Errors.Should().BeEmpty();
        }

        [Test]
        public void FailureResult_OneError_Errors_ReturnsError()
        {
            var expectedErrors = new List<ErrorMessage> { new ErrorMessage("Test") };

            var actual = Result.Failure(expectedErrors);

            actual.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Test]
        public void FailureResultT_OneError_Errors_ReturnsError()
        {
            var expectedErrors = new List<ErrorMessage> { new ErrorMessage("Test") };

            var actual = Result.Failure<string>(expectedErrors);

            actual.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Test]
        public void TwoDifferenceResults_AreNotEqual()
        {
            var success = Result.Success();
            var failure = Result.Failure(Array.Empty<ErrorMessage>());

            var actual = success.Equals(failure);

            actual.Should().BeFalse();
        }

        [Test]
        public void TwoDifferentResultsT_AreNotEqual()
        {
            var success = Result.Success("TestA");
            var failure = Result.Success("TestB");

            var actual = success.Equals(failure);

            actual.Should().BeFalse();
        }

        [Test]
        public void ToResult_ConvertSuccessResultT_ReturnsSuccessResult()
        {
            var sut = Result.Success("Test");
            
            var actual = sut.ToResult();

            actual.Should().Be(Result.Success());
        }

        [Test]
        public void ToResult_ConvertFailureResultT_ReturnsFailureResult()
        {
            List<ErrorMessage> expectedErrors = new List<ErrorMessage> { new ErrorMessage("TestErrorId") };
            var sut = Result.Failure<string>(expectedErrors);
            
            var actual = sut.ToResult();

            actual.Should().Be(Result.Failure(expectedErrors));
        }

        [Test]
        public void FailureResultT_ReturnsDefaultValue()
        {
            var actual = Result.Failure<int>(Array.Empty<ErrorMessage>());
            actual.Value.Should().Be(default);
        }
    }
}
