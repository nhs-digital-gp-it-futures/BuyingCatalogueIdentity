using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Constants;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Factories
{
    [TestFixture]
    public sealed class ApplicationUserClaimsPrincipalFactoryTests
    {
        [Test]
        public void CreateAsync_WhenNullUser_ShouldThrowException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var sut = ApplicationUserClaimsPrincipalFactoryBuilder
                    .Create()
                    .Build();

                await sut.CreateAsync(null);
            });
        }

        [TestCase("TestUserId", "TestUserName", "bf5bdc7d-9785-4464-8162-ab388ec22dd4", "Buyer")]
        [TestCase("", "TestUserName", "bf5bdc7d-9785-4464-8162-ab388ec22dd4", "Buyer")]
        [TestCase("TestUserId", "", "bf5bdc7d-9785-4464-8162-ab388ec22dd4", "Buyer")]
        [TestCase("TestUserId", "TestUserName", "bf5bdc7d-9785-4464-8162-ab388ec22dd4", "")]
        public async Task CreateAsync_WhenGivenClaimInfo_ShouldMatchExpectedClaimInfo(
            string expectedUserId, 
            string expectedUsername, 
            string expectedPrimaryOrganisationId, 
            string expectedOrganisationFunction)
        {
            Mock<IUserStore<ApplicationUser>> userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userStoreMock.Setup(x => x.GetUserIdAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUserId);

            userStoreMock.Setup(x => x.GetUserNameAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUsername);

            var userManager = UserManagerBuilder
                .Create()
                .WithUserStore(userStoreMock.Object)
                .Build();

            var sut = ApplicationUserClaimsPrincipalFactoryBuilder
                .Create()
                .WithUserManager(userManager)
                .Build();

            var actual = await sut.CreateAsync(new ApplicationUser
            {
                PrimaryOrganisationId = Guid.Parse(expectedPrimaryOrganisationId),
                OrganisationFunction = expectedOrganisationFunction
            });

            var expected = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>(ClaimTypes.NameIdentifier, expectedUserId),
                new KeyValuePair<string,string>(ClaimTypes.Name, expectedUsername),
                new KeyValuePair<string,string>(CustomClaimTypes.PrimaryOrganisationId, expectedPrimaryOrganisationId),
                new KeyValuePair<string,string>(CustomClaimTypes.OrganisationFunction, expectedOrganisationFunction)
            };

            actual.Claims.Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).Should().BeEquivalentTo(expected);
        }
    }
}
