using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    internal sealed class PasswordResetTokenTests
    {
        [Test]
        public void Constructor_String_ApplicationUser_InitializesExpectedMembers()
        {
            const string expectedToken = "TokenToken";
            var expectedUser = new ApplicationUser();

            var token = new PasswordResetToken(expectedToken, expectedUser);

            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }
    }
}
