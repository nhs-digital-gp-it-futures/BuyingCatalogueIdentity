using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
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
            var expectedUser = ApplicationUserBuilder.Create().Build();

            var token = new PasswordResetToken(expectedToken, expectedUser);

            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }
    }
}
