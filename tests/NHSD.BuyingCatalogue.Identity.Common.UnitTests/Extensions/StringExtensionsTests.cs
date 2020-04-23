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
        public void TrimAsync__NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                string input = null;
                input.TrimAsync();
            });
        }

        [Test]
        public void TrimAsync__InputContainingAsync_GetsTrimmed()
        {
            const string input = "FooAsync";
            const string expected = "Foo";
            string actual = input.TrimAsync();
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void TrimAsync__InputContainingAsync_GetsTrimmed_IgnoringCase()
        {
            const string input = "FooAsYnC";
            const string expected = "Foo";
            string actual = input.TrimAsync();
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void TrimAsync__InputNotContainingAsync_RemainsSame()
        {
            const string input = "Bar";
            const string expected = input;
            string actual = input.TrimAsync();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}

