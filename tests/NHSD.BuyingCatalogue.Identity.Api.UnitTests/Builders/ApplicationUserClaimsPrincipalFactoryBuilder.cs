using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NHSD.BuyingCatalogue.Identity.Api.Factories;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ApplicationUserClaimsPrincipalFactoryBuilder
    {
        private UserManager<ApplicationUser> _userManager;
        private IOptions<IdentityOptions> _identityOptions;

        public ApplicationUserClaimsPrincipalFactoryBuilder()
        {
            _userManager = UserManagerBuilder.Create().Build();
            _identityOptions = Options.Create(new IdentityOptions());
        }

        internal static ApplicationUserClaimsPrincipalFactoryBuilder Create()
        {
            return new ApplicationUserClaimsPrincipalFactoryBuilder();
        }

        internal ApplicationUserClaimsPrincipalFactoryBuilder WithUserManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            return this;
        }

        internal ApplicationUserClaimsPrincipalFactoryBuilder WithIdentityOptions(IOptions<IdentityOptions> identityOptions)
        {
            _identityOptions = identityOptions;
            return this;
        }

        internal ApplicationUserClaimsPrincipalFactory Build()
        {
            return new ApplicationUserClaimsPrincipalFactory(_userManager, _identityOptions);
        }
    }
}
