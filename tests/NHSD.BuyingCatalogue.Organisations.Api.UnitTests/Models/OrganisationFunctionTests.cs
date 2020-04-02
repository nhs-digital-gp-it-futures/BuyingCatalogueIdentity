using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class OrganisationFunctionTests
    {
        [Test]
        public void GetAllValues_ReturnsExpectedList()
        {
            var expected = new List<OrganisationFunction> { OrganisationFunction.Authority, OrganisationFunction.Buyer };

            var actual = typeof(OrganisationFunction).GetFields(BindingFlags.Public 
                                                                | BindingFlags.Static 
                                                                | BindingFlags.DeclaredOnly)
                .Select(fieldInfo => fieldInfo.GetValue(null) as OrganisationFunction);

            actual.Should().BeEquivalentTo(expected, config => config.WithoutStrictOrdering());
        }

        [TestCase("Authority", "Authority")]
        [TestCase("AUTHORITY", "Authority")]
        [TestCase("Buyer", "Buyer")]
        [TestCase("BUYER", "Buyer")]
        public void FromDisplayName_OrganisationFunctionName_ReturnsExpectedDisplayName(
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
        public void FromDisplayName_String_ReturnsNull(string displayNameInput)
        {
            var actual = OrganisationFunction.FromDisplayName(displayNameInput);

            actual.Should().BeNull();
        }

        [Test]
        public void FromDisplayName_NullDisplayName_ThrowsException()
        {
            static void FromDisplayName()
            {
                OrganisationFunction.FromDisplayName(null);
            }

            Assert.Throws<ArgumentNullException>(FromDisplayName);
        }

        [Test]
        public void TwoDifferentOrganisationFunction_AreNotEqual()
        {
            var actual = OrganisationFunction.Buyer.Equals(OrganisationFunction.Authority);
            actual.Should().BeFalse();
        }
    }
}
