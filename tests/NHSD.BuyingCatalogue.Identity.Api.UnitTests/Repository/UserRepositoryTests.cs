using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Repository
{
    [TestFixture]
    public sealed class UserRepositoryTests
    {
        [Test]
        public async Task UpdateAsync_UpdatingUserToDisabled_UpdatesUser()
        {
            var context = UserRepositoryTestContext.Setup();

            var user = ApplicationUserBuilder
                .Create()
                .WithDisabled(false)
                .Build();

            context.ContextInMemory.Users.Add(user);
            context.ContextInMemory.SaveChanges();

            user.MarkAsDisabled();

            await context.UserRepository.UpdateAsync(user);
            context.ContextInMemory.Users.First().Disabled.Should().BeTrue();
        }

        [Test]
        public void UpdateAsync_UserIsNull_ReturnsNullException()
        {
            static async Task TestAsync()
            {
                var context = UserRepositoryTestContext.Setup();
                await context.UserRepository.UpdateAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        }
    }

    internal sealed class UserRepositoryTestContext
    {
        public ApplicationDbContext ContextInMemory { get; set; }
        public UsersRepository UserRepository { get; set; }

        private UserRepositoryTestContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Add_writes_to_db");
            ContextInMemory = new ApplicationDbContext(optionsBuilder.Options);

            UserRepository = new UsersRepository(ContextInMemory);
        }

        internal static UserRepositoryTestContext Setup()
        {
            return new UserRepositoryTestContext();
        }
    }
}
