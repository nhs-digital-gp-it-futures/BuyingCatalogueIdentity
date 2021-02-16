using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ProfileDataRequestContextBuilder
    {
        private readonly Client client;
        private readonly string caller;
        private readonly IEnumerable<string> requestedClaimTypes;

        private string subjectId;

        internal ProfileDataRequestContextBuilder()
        {
            subjectId = string.Empty;
            client = new Client();
            caller = string.Empty;
            requestedClaimTypes = new List<string>();
        }

        internal static ProfileDataRequestContextBuilder Create()
        {
            return new();
        }

        internal ProfileDataRequestContextBuilder WithSubjectId(string id)
        {
            subjectId = id;
            return this;
        }

        internal ProfileDataRequestContext Build()
        {
            List<Claim> claims = new List<Claim>();

            if (subjectId is not null)
                claims.Add(new Claim(JwtClaimTypes.Subject, subjectId));

            return new ProfileDataRequestContext(
                new ClaimsPrincipal(new ClaimsIdentity(claims)),
                client,
                caller,
                requestedClaimTypes);
        }
    }
}
