using System;
using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Organisations.Api.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        private const string PrimaryOrganisationIdType = "primaryOrganisationId";

        internal static Guid GetPrimaryOrganisationId(this ClaimsPrincipal user)
        {
            return new Guid(user.FindFirst(PrimaryOrganisationIdType).Value);
        }
    }
}
