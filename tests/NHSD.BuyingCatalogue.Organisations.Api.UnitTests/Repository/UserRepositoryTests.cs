using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Repository
{
    [TestFixture]
    public sealed class UserRepositoryTests
    {
        [Test]
        public async Task UpdateAsync_UpdatingUserToDisabled_UpdatesUser()
        {
            var context = UserRepositoryTestContext.Setup();

            var user = ApplicationUserBuilder.Create().WithDisabled(false).BuildBuyer();
            context.ContextInMemory.Users.Add(user);
            context.ContextInMemory.SaveChanges();

            user.MarkAsDisabled();

            await context.UserRepository.UpdateAsync(user);
            context.ContextInMemory.Users.First().Disabled.Should().BeTrue();
        }

        [Test]
        public async Task UpdateAsync_UserIsNull_ReturnsNullExcepetion()
        {
            var context = UserRepositoryTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await context.UserRepository.UpdateAsync(null));
        }
    }
}
