using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class UserManagerBuilder
    {
        private IUserStore<ApplicationUser> _userStore;

        internal UserManagerBuilder()
        {
            _userStore = new Mock<IUserStore<ApplicationUser>>().Object;
        }

        internal static UserManagerBuilder Create()
        {
            return new UserManagerBuilder();
        }

        internal UserManagerBuilder WithUserStore(IUserStore<ApplicationUser> userStore)
        {
            _userStore = userStore;
            return this;
        }

        internal UserManager<ApplicationUser> Build()
        {
            var userManager = new UserManager<ApplicationUser>(_userStore, null, null, null, null, null, null, null, null);
            userManager.UserValidators.Add(new UserValidator<ApplicationUser>());
            userManager.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            return userManager;
        }
    }
}
