using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class ApplicationUserTests
    {
        [Test]
        public void UserName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername("  Test  ")
                .BuildBuyer();

            actual.UserName.Should().Be("Test");
        }

        [TestCase("Test", "TEST")]
        [TestCase("  Test   ", "TEST")]
        public void NormalizedUserName_SetUserName_ReturnsNormalizedUserName(string input, string expected)
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername(input)
                .BuildBuyer();

            actual.NormalizedUserName.Should().Be(expected);
        }

        [Test]
        public void NormalizedUserName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername("  Test  ")
                .BuildBuyer();

            actual.NormalizedUserName.Should().Be("TEST");
        }

        [Test]
        public void FirstName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithFirstName("  Dan  ")
                .BuildBuyer();

            actual.FirstName.Should().Be("Dan");
        }

        [Test]
        public void LastName_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithLastName("  Smith  ")
                .BuildBuyer();

            actual.LastName.Should().Be("Smith");
        }

        [Test]
        public void DisplayName_ReturnsExpectedValue()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithFirstName("Burt")
                .WithLastName("Reynolds")
                .BuildBuyer();

            actual.DisplayName.Should().Be("Burt Reynolds");
        }

        [Test]
        public void PhoneNumber_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithPhoneNumber("  0123456  ")
                .BuildBuyer();

            actual.PhoneNumber.Should().Be("0123456");
        }

        [Test]
        public void EmailAddress_Trimmed()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithEmailAddress("  a.b@c.com  ")
                .BuildBuyer();

            actual.Email.Should().Be("a.b@c.com");
        }

        [TestCase("a.b@c.com", "A.B@C.COM")]
        [TestCase("  a.b@c.com   ", "A.B@C.COM")]
        public void NormalizedEmail_SetEmail_ReturnsNormalizedEmail(string input, string expected)
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithEmailAddress(input)
                .BuildBuyer();

            actual.NormalizedEmail.Should().Be(expected);
        }

        [Test]
        public void MarkAsDisabled_DisabledIsTrue()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithDisabled(false)
                .BuildBuyer();

            actual.MarkAsDisabled();

            actual.Disabled.Should().BeTrue();
        }

        [Test]
        public void MarkCatalogueAgreementAsSigned_CatalogueAgreementSignedIsTrue()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithCatalogueAgreementSigned(false)
                .BuildBuyer();

            actual.MarkCatalogueAgreementAsSigned();

            actual.CatalogueAgreementSigned.Should().BeTrue();
        }

        [Test]
        public void Create_NullUserName_ThrowsException()
        {
            static void Test()
            {
                var actual = ApplicationUserBuilder
                    .Create()
                    .WithUsername(null)
                    .BuildBuyer();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Create_NullFirstName_ThrowsException()
        {
            static void Test()
            {
                var actual = ApplicationUserBuilder
                    .Create()
                    .WithFirstName(null)
                    .BuildBuyer();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Create_NullLastName_ThrowsException()
        {
            static void Test()
            {
                var actual = ApplicationUserBuilder
                    .Create()
                    .WithLastName(null)
                    .BuildBuyer();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Create_NullPhoneNumber_ThrowsException()
        {
            static void Test()
            {
                var actual = ApplicationUserBuilder
                    .Create()
                    .WithPhoneNumber(null)
                    .BuildBuyer();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void Create_NullEmailAddress_ThrowsException()
        {
            static void Test()
            {
                var actual = ApplicationUserBuilder
                    .Create()
                    .WithEmailAddress(null)
                    .BuildBuyer();
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public void CreateBuyer_ReturnsBuyerOrganisationFunction()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .BuildBuyer().OrganisationFunction;

            actual.Should().Be(OrganisationFunction.Buyer);
        }
    }
}
