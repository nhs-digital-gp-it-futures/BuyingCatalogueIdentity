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
    [Parallelizable(ParallelScope.All)]
    internal static class UserRepositoryTests
    {
        [Test]
        public static async Task UpdateAsync_UpdatingUserToDisabled_UpdatesUser()
        {
            var context = UserRepositoryTestContext.Setup();

            var user = ApplicationUserBuilder
                .Create()
                .WithDisabled(false)
                .Build();

            await context.ContextInMemory.Users.AddAsync(user);
            await context.ContextInMemory.SaveChangesAsync();

            user.MarkAsDisabled();

            await context.UserRepository.UpdateAsync(user);
            context.ContextInMemory.Users.First().Disabled.Should().BeTrue();
        }

        [Test]
        public static void UpdateAsync_UserIsNull_ReturnsNullException()
        {
            var context = UserRepositoryTestContext.Setup();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await context.UserRepository.UpdateAsync(null));
        }

        private sealed class UserRepositoryTestContext
        {
            private UserRepositoryTestContext()
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("Add_writes_to_db");
                ContextInMemory = new ApplicationDbContext(optionsBuilder.Options);

                UserRepository = new UsersRepository(ContextInMemory);
            }

            public ApplicationDbContext ContextInMemory { get; }

            public UsersRepository UserRepository { get; }

            internal static UserRepositoryTestContext Setup()
            {
                return new();
            }
        }
    }
}
