using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ApplicationUserTests
    {
        [Test]
        public static void UserName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername("  Test  ")
                .Build();

            actual.UserName.Should().Be("Test");
        }

        [TestCase("Test", "TEST")]
        [TestCase("  Test   ", "TEST")]
        public static void NormalizedUserName_SetUserName_ReturnsNormalizedUserName(string input, string expected)
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername(input)
                .Build();

            actual.NormalizedUserName.Should().Be(expected);
        }

        [Test]
        public static void NormalizedUserName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername("  Test  ")
                .Build();

            actual.NormalizedUserName.Should().Be("TEST");
        }

        [Test]
        public static void FirstName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithFirstName("  Dan  ")
                .Build();

            actual.FirstName.Should().Be("Dan");
        }

        [Test]
        public static void LastName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithLastName("  Smith  ")
                .Build();

            actual.LastName.Should().Be("Smith");
        }

        [Test]
        public static void DisplayName_ReturnsExpectedValue()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithFirstName("Burt")
                .WithLastName("Reynolds")
                .Build();

            actual.DisplayName.Should().Be("Burt Reynolds");
        }

        [Test]
        public static void PhoneNumber_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithPhoneNumber("  0123456  ")
                .Build();

            actual.PhoneNumber.Should().Be("0123456");
        }

        [Test]
        public static void EmailAddress_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithEmailAddress("  a.b@c.com  ")
                .Build();

            actual.Email.Should().Be("a.b@c.com");
        }

        [TestCase("a.b@c.com", "A.B@C.COM")]
        [TestCase("  a.b@c.com   ", "A.B@C.COM")]
        public static void NormalizedEmail_SetEmail_ReturnsNormalizedEmail(string input, string expected)
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(input)
                .Build();

            actual.NormalizedEmail.Should().Be(expected);
        }

        [Test]
        public static void MarkAsDisabled_DisabledIsTrue()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithDisabled(false)
                .Build();

            actual.MarkAsDisabled();

            actual.Disabled.Should().BeTrue();
        }

        [Test]
        public static void MarkCatalogueAgreementAsSigned_CatalogueAgreementSignedIsTrue()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithCatalogueAgreementSigned(false)
                .Build();

            actual.MarkCatalogueAgreementAsSigned();

            actual.CatalogueAgreementSigned.Should().BeTrue();
        }

        [Test]
        public static void Create_NullUserName_ThrowsException()
        {
            static void Test()
            {
                ApplicationUserBuilder
                    .Create()
                    .WithUsername(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void Create_NullFirstName_ThrowsException()
        {
            static void Test()
            {
                ApplicationUserBuilder
                    .Create()
                    .WithFirstName(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void Create_NullLastName_ThrowsException()
        {
            static void Test()
            {
                ApplicationUserBuilder
                    .Create()
                    .WithLastName(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void Create_NullPhoneNumber_ThrowsException()
        {
            static void Test()
            {
                ApplicationUserBuilder
                    .Create()
                    .WithPhoneNumber(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void Create_NullEmailAddress_ThrowsException()
        {
            static void Test()
            {
                ApplicationUserBuilder
                    .Create()
                    .WithEmailAddress(null)
                    .Build();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void CreateBuyer_ReturnsBuyerOrganisationFunction()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithOrganisationFunction(OrganisationFunction.Buyer)
                .Build().OrganisationFunction;

            actual.Should().Be(OrganisationFunction.Buyer);
        }

        [Test]
        public static void CreateAuthority_ReturnsAuthorityOrganisationFunction()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithOrganisationFunction(OrganisationFunction.Authority)
                .Build().OrganisationFunction;

            actual.Should().Be(OrganisationFunction.Authority);
        }
    }
}
