using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class LoginViewModelTests
    {
        private const string Password = "Password";
        private const string Username = "User";

        [Test]
        [TestCase(null, null, LoginViewModel.ErrorMessages.PasswordRequired, LoginViewModel.ErrorMessages.UsernameRequired)]
        [TestCase(Password, null, LoginViewModel.ErrorMessages.UsernameRequired)]
        [TestCase(null, Username, LoginViewModel.ErrorMessages.PasswordRequired)]
        [TestCase(Password, Username)]
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
