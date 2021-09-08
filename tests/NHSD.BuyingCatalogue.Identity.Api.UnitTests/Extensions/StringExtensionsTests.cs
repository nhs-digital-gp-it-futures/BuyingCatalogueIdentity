using System;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class StringExtensionsTests
    {
        [Test]
        public static void ExtractCookieCreationDate_ValidString_ReturnsExpectedDateTime()
        {
            var expected = DateTime.Now.AddDays(-42);

            var actual = expected.ToCookieDataString().ExtractCookieCreationDate();

            actual.Should().BeCloseTo(expected, TimeSpan.FromSeconds(30));
        }

        [Test]
        public static void ExtractCookieCreationDate_InvalidCreationDateString_ReturnsNull()
        {
            var cookieData = new CookieData { CookieValue = true, CreationDate = null };

            var actual = JsonConvert.SerializeObject(cookieData).ExtractCookieCreationDate();

            actual.Should().Be(DateTime.MinValue);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("some-string")]
        [TestCase("{\"CookieValue\":true,\"CreationDate\":'invalid'}")]
        [TestCase("{\"cookievalue\":abc,\"creationdate\":1633439729761.933}")]
        public static void ExtractCookieCreationDate_StringInvalid_ReturnsMinDateTime(string invalid)
        {
            var actual = invalid.ExtractCookieCreationDate();

            actual.Should().Be(DateTime.MinValue);
        }

        [Test]
        public static void ToCookieDataString_DateTimeInput_ResultAsExpected()
        {
            var dateTime = DateTime.Now.AddMonths(4);
            var expected = JsonConvert.SerializeObject(
                new CookieData
                {
                    CookieValue = true,
                    CreationDate = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                        .TotalMilliseconds,
                });

            var actual = dateTime.ToCookieDataString();

            actual.Should().Be(expected);
        }
    }
}
