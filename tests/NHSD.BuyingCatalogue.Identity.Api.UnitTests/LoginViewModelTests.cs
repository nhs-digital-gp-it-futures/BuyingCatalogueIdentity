using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    [TestFixture]
    internal sealed class LoginViewModelTests
    {
        [Test]
        [TestCase(null, null, "Enter your password", "Enter your email address")]
        [TestCase("Password", null, "Enter your email address")]
        [TestCase(null, "User", "Enter your password")]
        [TestCase("Password", "User")]
        public void InvalidModel_HasExpectedValidationErrorsFoo(string password, string username, params string[] expectedErrors)
        {
            var errors = new List<ValidationResult>();
            var model = new LoginViewModel { Password = password, Username = username };

            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), errors);

            isValid.Should().Be(expectedErrors.Length == 0);
            errors.Count.Should().Be(expectedErrors.Length);
            errors.Select(v => v.ErrorMessage).SequenceEqual(expectedErrors).Should().BeTrue();
        }
    }
}
