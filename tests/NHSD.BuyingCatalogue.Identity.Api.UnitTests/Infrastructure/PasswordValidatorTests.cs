using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Infrastructure
{
    [TestFixture]
    public sealed class PasswordValidatorTests
    {
        [TestCase("Pass123123")]
        [TestCase("Pass$$$$$$")]
        [TestCase("pass$$$123")]
        [TestCase("PASS$$$123")]
        public void ValidateAsync_ValidPassword_ReturnsSuccessfulIdentityResult(string password)
        {
            var validator = new PasswordValidator();
            var result = validator.ValidateAsync(null, null, password);
            result.Result.Succeeded.Should().BeTrue();
        }

        [TestCase("")]
        [TestCase("Pass12312")]
        [TestCase("pass123123")]
        [TestCase("PASS123123")]
        [TestCase("$$$$123123")]
        [TestCase("pass$$$$$$")]
        [TestCase("PASS$$$$$$")]
        [TestCase("PASSonetwothree")]
        public void ValidateAsync_InvalidPassword_ReturnsFailureIdentityResult(string password)
        {
            var validator = new PasswordValidator();
            var result = validator.ValidateAsync(null, null, password);

            result.Result.Succeeded.Should().BeFalse();
            result.Result.Errors.Count().Should().Be(1);
            var error = result.Result.Errors.First();
            error.Code.Should().Be(PasswordValidator.InvalidPasswordCode);
            error.Description.Should().Be(PasswordValidator.PasswordConditionsNotMet);
        }
    }
}
