using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class ApplicationUserProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

        public ApplicationUserProfileService(
            UserManager<ApplicationUser> userManager, 
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);
            if (user is object)
            {
                var userClaims = await GetClaimsFromUserAsync(user);
                context.AddRequestedClaims(userClaims);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.ThrowIfNull(nameof(context));

            ClaimsPrincipal subject = context.Subject;
            ApplicationUser user = await GetApplicationUserAsync(subject);

            context.IsActive = user is object;
        }

        private async Task<ApplicationUser> GetApplicationUserAsync(ClaimsPrincipal subject)
        {
            subject.ThrowIfNull(nameof(subject));

            string subjectId = subject.GetSubjectId();
            return await _userManager.FindByIdAsync(subjectId);
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUserAsync(ApplicationUser user)
        {
            ClaimsPrincipal claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(user);
            return claimsPrincipal.Claims.ToList();
        }
    }
}
