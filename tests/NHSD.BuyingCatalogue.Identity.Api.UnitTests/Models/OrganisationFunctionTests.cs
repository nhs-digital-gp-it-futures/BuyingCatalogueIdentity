using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationFunctionTests
    {
        [Test]
        public static void GetAllValues_ReturnsExpectedList()
        {
            var expected = new List<OrganisationFunction> { OrganisationFunction.Authority, OrganisationFunction.Buyer };

            var actual = typeof(OrganisationFunction)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(fieldInfo => fieldInfo.GetValue(null) as OrganisationFunction);

            actual.Should().BeEquivalentTo(expected, config => config.WithoutStrictOrdering());
        }

        [TestCase("Authority", "Authority")]
        [TestCase("AUTHORITY", "Authority")]
        [TestCase("Buyer", "Buyer")]
        [TestCase("BUYER", "Buyer")]
        public static void FromDisplayName_OrganisationFunctionName_ReturnsExpectedDisplayName(
            string displayNameInput,
            string expectedDisplayName)
        {
            var actual = OrganisationFunction.FromDisplayName(displayNameInput);

            actual.DisplayName.Should().Be(expectedDisplayName);
        }

        [TestCase("")]
        [TestCase("Authority ")]
        [TestCase("Buyer ")]
        [TestCase("  Authority ")]
        [TestCase("1234566")]
        public static void FromDisplayName_String_ReturnsNull(string displayNameInput)
        {
            var actual = OrganisationFunction.FromDisplayName(displayNameInput);

            actual.Should().BeNull();
        }

        [Test]
        public static void FromDisplayName_NullDisplayName_ThrowsException()
        {
            static void FromDisplayName()
            {
                OrganisationFunction.FromDisplayName(null);
            }

            Assert.Throws<ArgumentNullException>(FromDisplayName);
        }

        [Test]
        public static void TwoDifferentOrganisationFunction_AreNotEqual()
        {
            var actual = OrganisationFunction.Buyer.Equals(OrganisationFunction.Authority);
            actual.Should().BeFalse();
        }
    }
}
