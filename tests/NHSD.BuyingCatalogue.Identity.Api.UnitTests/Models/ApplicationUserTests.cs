using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Models
{
    [TestFixture]
    internal sealed class ApplicationUserTests
    {
        [Test]
        public void DisplayName_ReturnsExpectedValue()
        {
            var user = new ApplicationUser
            {
                FirstName = "Edgar",
                LastName = "Poe",
            };

            user.DisplayName.Should().Be("Edgar Poe");
        }
    }
}
