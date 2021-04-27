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
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Repositories;
using NHSD.BuyingCatalogue.Identity.Common.Constants;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class ProfileService : IProfileService
    {
        private static readonly IDictionary<OrganisationFunction, IEnumerable<Claim>> OrganisationFunctionClaims =
            new Dictionary<OrganisationFunction, IEnumerable<Claim>>
            {
                {
                    OrganisationFunction.Authority,
                    new List<Claim>
                    {
                        new(ApplicationClaimTypes.Organisation, ApplicationPermissions.Manage),
                        new(ApplicationClaimTypes.Account, ApplicationPermissions.Manage),
                    }
                },
                {
                    OrganisationFunction.Buyer,
                    new List<Claim>
                    {
                        new(ApplicationClaimTypes.Organisation, ApplicationPermissions.View),
                        new(ApplicationClaimTypes.Ordering, ApplicationPermissions.Manage),
                    }
                },
            };

        private readonly IUsersRepository applicationUserRepository;

        private readonly IOrganisationRepository organisationRepository;

        public ProfileService(IUsersRepository applicationUserRepository, IOrganisationRepository organisationRepository)
        {
            this.applicationUserRepository = applicationUserRepository ??
                throw new ArgumentNullException(nameof(applicationUserRepository));

            this.organisationRepository = organisationRepository ??
                throw new ArgumentNullException(nameof(organisationRepository));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);

            if (user is not null)
            {
                var claims = GetClaimsFromUser(user);
                claims.AddRange(await GetPrimaryOrganisationClaims(user.PrimaryOrganisationId));

                context.IssuedClaims = claims;
            }
        }

        public async Task<IEnumerable<Claim>> GetPrimaryOrganisationClaims(Guid primaryOrganisationId)
        {
            var claims = new List<Claim>();

            var primaryOrganisation = await organisationRepository.GetByIdAsync(primaryOrganisationId);

            if (primaryOrganisation is null)
                return claims;

            if (!primaryOrganisation.Name.IsNullOrEmpty())
            {
                claims.Add(new Claim(ApplicationClaimTypes.PrimaryOrganisationName, primaryOrganisation.Name));
            }

            claims.AddRange(primaryOrganisation.RelatedOrganisations.Select(
                ro => new Claim(ApplicationClaimTypes.RelatedOrganisationId, ro.OrganisationId.ToString())));

            return claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ApplicationUser user = await GetApplicationUserAsync(context.Subject);

            context.IsActive = user is not null;
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
            if (string.IsNullOrWhiteSpace(email))
                yield break;

            yield return new Claim(JwtClaimTypes.Email, email);
            yield return new Claim(
                JwtClaimTypes.EmailVerified,
                user.EmailConfirmed ? "true" : "false",
                ClaimValueTypes.Boolean);
        }

        private static IEnumerable<Claim> GetIdClaims(ApplicationUser user)
        {
            string userId = user.Id;
            if (!string.IsNullOrWhiteSpace(userId))
                yield return new Claim(JwtClaimTypes.Subject, userId);

            string username = user.UserName;
            if (string.IsNullOrWhiteSpace(username))
                yield break;

            yield return new Claim(JwtClaimTypes.PreferredUserName, username);
            yield return new Claim(JwtRegisteredClaimNames.UniqueName, username);
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

            if (!OrganisationFunctionClaims.TryGetValue(organisationFunction, out IEnumerable<Claim> organisationClaims))
                yield break;

            foreach (Claim claim in organisationClaims)
            {
                yield return claim;
            }
        }

        private async Task<ApplicationUser> GetApplicationUserAsync(ClaimsPrincipal subject)
        {
            if (subject is null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            string subjectId = subject.GetSubjectId();
            return await applicationUserRepository.GetByIdAsync(subjectId);
        }
    }
}
