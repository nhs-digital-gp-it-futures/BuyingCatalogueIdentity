using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Factories
{
    public sealed class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager, 
            IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            user.ThrowIfNull(nameof(user));

            ClaimsIdentity claimsIdentity = await base.GenerateClaimsAsync(user);

            claimsIdentity.AddClaim(new Claim("primaryOrganisation", user.PrimaryOrganisationId.ToString()));
            claimsIdentity.AddClaim(new Claim("organisationFunction", user.OrganisationFunction ?? ""));
            claimsIdentity.AddClaim(new Claim("email", user.Email ?? ""));

            return claimsIdentity;
        }
    }
}
