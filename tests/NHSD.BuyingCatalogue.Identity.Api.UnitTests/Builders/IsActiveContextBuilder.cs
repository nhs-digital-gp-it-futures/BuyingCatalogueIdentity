using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class IsActiveContextBuilder
    {
        private readonly string caller;
        private string subjectId;

        private IsActiveContextBuilder()
        {
            subjectId = string.Empty;
            caller = "Test";
        }

        internal static IsActiveContextBuilder Create()
        {
            return new();
        }

        internal IsActiveContextBuilder WithSubjectId(string id)
        {
            subjectId = id;
            return this;
        }

        internal IsActiveContext Build()
        {
            List<Claim> claims = new List<Claim>();

            if (subjectId is not null)
                claims.Add(new Claim(JwtClaimTypes.Subject, subjectId));

            return new IsActiveContext(
                new ClaimsPrincipal(new ClaimsIdentity(claims)),
                new Client(),
                caller);
        }
    }
}
