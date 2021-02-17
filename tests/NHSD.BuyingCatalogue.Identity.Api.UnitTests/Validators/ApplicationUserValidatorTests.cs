using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Identity.Api.Validators;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Validators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ApplicationUserValidatorTests
    {
        [Test]
        public static async Task ValidateAsync_ValidApplicationUser_ReturnsSuccess()
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .Build();

            var actual = await sut.ValidateAsync(user);

            actual.Should().Be(Result.Success());
        }

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidFirstNameCases))]
        public static async Task ValidateAsync_WithFirstName_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithFirstName(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(ApplicationUser.FirstName))));
            actual.Should().Be(expected);
        }

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidLastNameCases))]
        public static async Task ValidateAsync_WithLastName_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithLastName(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(ApplicationUser.LastName))));
            actual.Should().Be(expected);
        }

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidPhoneNumberCases))]
        public static async Task ValidateAsync_WithPhoneNumber_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithPhoneNumber(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(ApplicationUser.PhoneNumber))));
            actual.Should().Be(expected);
        }

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidEmailTestCases))]
        public static async Task ValidateAsync_WithEmailAddress_ReturnsFailure(string input, string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, "EmailAddress")));
            actual.Should().Be(expected);
        }

        [Test]
        public static async Task ValidateAsync_DuplicateEmailAddress_ReturnsFailure()
        {
            const string duplicateEmailAddress = "duplicate@email.com";

            var context = ApplicationUserValidatorTestContext.Setup();
            context.ApplicationUserByEmail = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .Build();

            var sut = context.ApplicationUserValidator;

            var user = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(new List<ErrorDetails> { new("EmailAlreadyExists", "EmailAddress") });
            actual.Should().Be(expected);
        }

        [Test]
        public static void Constructor_NullUserRepository_ThrowsException()
        {
            static void Test()
            {
                _ = new ApplicationUserValidator(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void ValidateAsync_NullApplicationUser_ThrowsException()
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ValidateAsync(null));
        }

        private static class TestContextTestCaseData
        {
            internal static IEnumerable<TestCaseData> InvalidFirstNameCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "FirstNameRequired" });
                    yield return new TestCaseData("  ", new[] { "FirstNameRequired" });
                    yield return new TestCaseData(new string('a', 101), new[] { "FirstNameTooLong" });
                }
            }

            internal static IEnumerable<TestCaseData> InvalidLastNameCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "LastNameRequired" });
                    yield return new TestCaseData("  ", new[] { "LastNameRequired" });
                    yield return new TestCaseData(new string('a', 101), new[] { "LastNameTooLong" });
                }
            }

            internal static IEnumerable<TestCaseData> InvalidPhoneNumberCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "PhoneNumberRequired" });
                    yield return new TestCaseData("  ", new[] { "PhoneNumberRequired" });
                    yield return new TestCaseData(new string('p', 36), new[] { "PhoneNumberTooLong" });
                }
            }

            internal static IEnumerable<TestCaseData> InvalidEmailTestCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "EmailRequired" });
                    yield return new TestCaseData("  ", new[] { "EmailRequired" });
                    yield return new TestCaseData($"a@{new string('b', 255)}", new[] { "EmailTooLong" });
                    yield return new TestCaseData("test", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("test@", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("@test", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("@", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("bobsmith@test@com", new[] { "EmailInvalidFormat" });
                }
            }
        }

        private sealed class ApplicationUserValidatorTestContext
        {
            private ApplicationUserValidatorTestContext()
            {
                UsersRepositoryMock = new Mock<IUsersRepository>();
                UsersRepositoryMock
                    .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => ApplicationUserByEmail);

                ApplicationUserValidator = new ApplicationUserValidator(UsersRepositoryMock.Object);
            }

            internal ApplicationUserValidator ApplicationUserValidator { get; }

            internal ApplicationUser ApplicationUserByEmail { get; set; }

            private Mock<IUsersRepository> UsersRepositoryMock { get; }

            public static ApplicationUserValidatorTestContext Setup()
            {
                return new();
            }
        }
    }
}
