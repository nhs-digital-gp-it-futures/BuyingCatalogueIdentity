using System;
using System.Linq;
using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Organisations.Api.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private const string PrimaryOrganisationIdType = "primaryOrganisationId";
        private const string RelatedOrganisationIdType = "relatedOrganisationId";

        internal static Guid GetPrimaryOrganisationId(this ClaimsPrincipal user)
        {
            var primaryOrganisationIdClaim = user.FindFirst(PrimaryOrganisationIdType);

            return primaryOrganisationIdClaim is null
                ? Guid.Empty
                : new Guid(primaryOrganisationIdClaim.Value);
        }

        internal static bool IsAuthorisedForOrganisation(this ClaimsPrincipal user, string id)
        {
            var userAuthorisedOrganisations = user.FindAll(RelatedOrganisationIdType)
                .Select(c => c.Value)
                .Append(user.FindFirstValue(PrimaryOrganisationIdType))
                .Where(s => !string.IsNullOrEmpty(s));

            return userAuthorisedOrganisations.Any(s => s.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
