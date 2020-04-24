using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class StringExtensionsTests
    {
        [Test]
        public void TrimAsync_NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                string input = null;
                input.TrimAsync();
            });
        }

        [Test]
        public void TrimController_NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                string input = null;
                input.TrimController();
            });
        }

        [TestCase("FooAsync", "Foo")]
        [TestCase("Fooasync", "Foo")]
        [TestCase("Foo", "Foo")]
        public void TrimAsync_Tests(string input, string expected)
        {
            string actual = input.TrimAsync();
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("BarController", "Bar")]
        [TestCase("Barcontroller", "Barcontroller")]
        [TestCase("Bar", "Bar")]
        public void TrimController_Tests(string input, string expected)
        {
            string actual = input.TrimController();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
