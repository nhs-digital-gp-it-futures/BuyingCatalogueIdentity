using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class StringExtensionsTests
    {
        [Test]
        public static void TrimAsync_NullInput_Throws()
        {
            string input = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                input.TrimAsync();
            });
        }

        [Test]
        public static void TrimController_NullInput_Throws()
        {
            string input = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                input.TrimController();
            });
        }

        // ReSharper disable once StringLiteralTypo
        [TestCase("FooAsync", "Foo")]
        [TestCase("Fooasync", "Foo")]
        [TestCase("Foo", "Foo")]
        public static void TrimAsync_Tests(string input, string expected)
        {
            string actual = input.TrimAsync();
            actual.Should().BeEquivalentTo(expected);
        }

        // ReSharper disable StringLiteralTypo
        [TestCase("Barcontroller", "Barcontroller")] // ReSharper restore StringLiteralTypo
        [TestCase("BarController", "Bar")]
        [TestCase("Bar", "Bar")]
        public static void TrimController_Tests(string input, string expected)
        {
            string actual = input.TrimController();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
