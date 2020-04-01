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
                .Build();

            actual.DisplayName.Should().Be("Burt Reynolds");
        }
    }
}
