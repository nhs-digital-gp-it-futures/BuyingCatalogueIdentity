using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using NHSD.BuyingCatalogue.Identity.Api.Constants;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly IUserRepository _applicationUserRepository;
        
        public ProfileService(IUserRepository applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository ?? throw new ArgumentNullException(nameof(applicationUserRepository));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);
            if (user is object)
            {
                var userClaims = GetClaimsFromUser(user);
                context.IssuedClaims = userClaims.ToList();
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
            return await _applicationUserRepository.FindByIdAsync(subjectId);
        }

        private IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(ApplicationClaimTypes.PrimaryOrganisationId, user.PrimaryOrganisationId.ToString())
            };

            string username = user.UserName;
            if (!string.IsNullOrWhiteSpace(username))
            {
                claims.Add(new Claim(JwtClaimTypes.PreferredUserName, username));
                claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, username));
            }

            string organisationFunction = user.OrganisationFunction;
            if (!string.IsNullOrWhiteSpace(organisationFunction))
            {
                claims.Add(new Claim(ApplicationClaimTypes.OrganisationFunction, organisationFunction));

                if (organisationFunction.Equals("Authority", StringComparison.OrdinalIgnoreCase))
                {
                    claims.Add(new Claim(ApplicationClaimTypes.Organisation, "view"));
                }
            }

            string email = user.Email;
            if (!string.IsNullOrWhiteSpace(email))
            {
                claims.Add(new Claim(JwtClaimTypes.Email, email));
                claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean));
            }

            return claims;
        }
    }
}
