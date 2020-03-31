using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
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
            var user = new ApplicationUser
            {
                FirstName = "Burt",
                LastName = "Reynolds",
            };

            user.DisplayName.Should().Be("Burt Reynolds");
        }
    }
}
