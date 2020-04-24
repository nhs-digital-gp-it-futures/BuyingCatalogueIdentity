using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly IUsersRepository _applicationUserRepository;

        private static readonly IDictionary<OrganisationFunction, IEnumerable<Claim>> _organisationFunctionClaims =
            new Dictionary<OrganisationFunction, IEnumerable<Claim>>
            {
                {
                    OrganisationFunction.Authority,
                    new List<Claim>
                    {
                        new Claim(ApplicationClaimTypes.Organisation, ApplicationPermissions.Manage),
                        new Claim(ApplicationClaimTypes.Account, ApplicationPermissions.Manage)
                    }
                },
                {
                    OrganisationFunction.Buyer,  new List<Claim>
                    {
                        new Claim(ApplicationClaimTypes.Ordering,ApplicationPermissions.Manage)
                    }
                }
            };

        public ProfileService(IUsersRepository applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository ??
                throw new ArgumentNullException(nameof(applicationUserRepository));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);
            if (user is object)
            {
                context.IssuedClaims = GetClaimsFromUser(user);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.ThrowIfNull(nameof(context));

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);

            context.IsActive = user is object;
        }

        private async Task<ApplicationUser> GetApplicationUserAsync(ClaimsPrincipal subject)
        {
            subject.ThrowIfNull(nameof(subject));

            string subjectId = subject.GetSubjectId();
            return await _applicationUserRepository.GetByIdAsync(subjectId);
        }

        private static List<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>();

            claims.AddRange(GetIdClaims(user));
            claims.AddRange(GetNameClaims(user));
            claims.AddRange(GetEmailClaims(user));
            claims.AddRange(GetOrganisationClaims(user));

            return claims;
        }

        private static IEnumerable<Claim> GetEmailClaims(ApplicationUser user)
        {
            string email = user.Email;
            if (!string.IsNullOrWhiteSpace(email))
            {
                yield return new Claim(JwtClaimTypes.Email, email);
                yield return new Claim(
                    JwtClaimTypes.EmailVerified, 
                    user.EmailConfirmed ? "true" : "false",
                    ClaimValueTypes.Boolean);
            }
        }

        private static IEnumerable<Claim> GetIdClaims(ApplicationUser user)
        {
            string userId = user.Id;
            if (!string.IsNullOrWhiteSpace(userId))
                yield return new Claim(JwtClaimTypes.Subject, userId);

            string username = user.UserName;
            if (!string.IsNullOrWhiteSpace(username))
            {
                yield return new Claim(JwtClaimTypes.PreferredUserName, username);
                yield return new Claim(JwtRegisteredClaimNames.UniqueName, username);
            }
        }

        private static IEnumerable<Claim> GetNameClaims(ApplicationUser user)
        {
            string firstName = user.FirstName;
            if (!string.IsNullOrWhiteSpace(firstName))
                yield return new Claim(JwtClaimTypes.GivenName, firstName);

            string lastName = user.LastName;
            if (!string.IsNullOrWhiteSpace(lastName))
                yield return new Claim(JwtClaimTypes.FamilyName, lastName);

            string name = $"{firstName?.Trim()} {lastName?.Trim()}".Trim();
            if (!string.IsNullOrWhiteSpace(name))
                yield return new Claim(JwtClaimTypes.Name, name);
        }

        private static IEnumerable<Claim> GetOrganisationClaims(ApplicationUser user)
        {
            yield return new Claim(ApplicationClaimTypes.PrimaryOrganisationId, user.PrimaryOrganisationId.ToString());

            OrganisationFunction organisationFunction = user.OrganisationFunction;

            yield return new Claim(ApplicationClaimTypes.OrganisationFunction, organisationFunction.DisplayName);

            if (_organisationFunctionClaims.TryGetValue(organisationFunction, out IEnumerable<Claim> organisationClaims))
            {
                foreach (Claim claim in organisationClaims)
                {
                    yield return claim;
                }
            }
        }
    }
}
