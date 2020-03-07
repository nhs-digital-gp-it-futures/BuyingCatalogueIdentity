using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class ProfileService : IProfileService
    {
        private const string PrimaryOrganisationClaimKey = "primaryOrganisation";
        private const string OrganisationFunctionClaimKey = "organisationFunction";

        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);
            if (user is object)
            {
                context.IssuedClaims = GetClaimsFromUser(user).ToList();
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.ThrowIfNull(nameof(context));

            ClaimsPrincipal subject = context.Subject;
            ApplicationUser user = await GetApplicationUserAsync(subject);

            context.IsActive = false;

            if (user is object)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    string securityStamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (securityStamp is object)
                    {
                        string securityStampFromDatabase = await _userManager.GetSecurityStampAsync(user);
                        if (!securityStamp.Equals(securityStampFromDatabase, StringComparison.Ordinal))
                        {
                            return;
                        }
                    }
                }

                context.IsActive = true;
            }
        }

        private async Task<ApplicationUser> GetApplicationUserAsync(ClaimsPrincipal subject)
        {
            subject.ThrowIfNull(nameof(subject));

            string subjectId = subject.Claims
                .FirstOrDefault(claim => JwtRegisteredClaimNames.Sub.Equals(claim.Type, StringComparison.Ordinal))?.Value;

            return await _userManager.FindByIdAsync(subjectId);
        }

        private IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(PrimaryOrganisationClaimKey, user.PrimaryOrganisationId.ToString())
            };

            if (string.IsNullOrWhiteSpace(user.OrganisationFunction))
            {
                claims.Add(new Claim(OrganisationFunctionClaimKey, user.OrganisationFunction));
            }

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            return claims;
        }
    }
}
