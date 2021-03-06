﻿using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Services;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PasswordResetTokenTests
    {
        [Test]
        public static void Constructor_String_ApplicationUser_InitializesExpectedMembers()
        {
            const string expectedToken = "TokenToken";
            var expectedUser = ApplicationUserBuilder.Create().Build();

            var token = new PasswordResetToken(expectedToken, expectedUser);

            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }
    }
}
