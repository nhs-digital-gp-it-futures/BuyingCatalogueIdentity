using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class IsActiveContextBuilder
    {
        private string _subjectId;
        private readonly string _caller;

        private IsActiveContextBuilder()
        {
            _subjectId = string.Empty;
            _caller = "Test";
        }

        internal static IsActiveContextBuilder Create()
        {
            return new IsActiveContextBuilder();
        }

        internal IsActiveContextBuilder WithSubjectId(string subjectId)
        {
            _subjectId = subjectId;
            return this;
        }

        internal IsActiveContext Build()
        {
            List<Claim> claims = new List<Claim>();

            if (_subjectId is object)
                claims.Add(new Claim(JwtClaimTypes.Subject, _subjectId));

            return new IsActiveContext(
                new ClaimsPrincipal(new ClaimsIdentity(claims)), 
                new Client(), 
                _caller);
        }
    }
}
