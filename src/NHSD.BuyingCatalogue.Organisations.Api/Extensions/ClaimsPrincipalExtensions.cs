using System;
using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Organisations.Api.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private const string PrimaryOrganisationIdType = "primaryOrganisationId";

        internal static Guid GetPrimaryOrganisationId(this ClaimsPrincipal user)
        {
            var primaryOrganisationIdClaim = user.FindFirst(PrimaryOrganisationIdType);

            return primaryOrganisationIdClaim is null
                ? Guid.Empty
                : new Guid(primaryOrganisationIdClaim.Value);
        }
    }
}
