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
        public void UserName_SetsNormalizedUserName()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithUsername("Test")
                .BuildBuyer();

            actual.NormalizedUserName.Should().Be("TEST");
        }

        [Test]
        public void EmailAddress_SetsNormalizedEmail()
        {
            var actual = ApplicationUserBuilder
                .Create()
                .WithEmailAddress("a.b@c.com")
                .BuildBuyer();

            actual.NormalizedEmail.Should().Be("A.B@C.COM");
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
