using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class ProfileDataRequestContextBuilder
    {
        private string _subjectId;
        private readonly Client _client;
        private readonly string _caller;
        private IEnumerable<string> _requestedClaimTypes;

        internal ProfileDataRequestContextBuilder()
        {
            _subjectId = string.Empty;
            _client = new Client();
            _caller = string.Empty;
            _requestedClaimTypes = new List<string>();
        }

        internal static ProfileDataRequestContextBuilder Create()
        {
            return new ProfileDataRequestContextBuilder();
        }

        internal ProfileDataRequestContextBuilder WithSubjectId(string subjectId)
        {
            _subjectId = subjectId;
            return this;
        }

        internal ProfileDataRequestContextBuilder WithRequestedClaimTypes(params string[] requestedClaimTypes)
        {
            _requestedClaimTypes = requestedClaimTypes;
            return this;
        }

        internal ProfileDataRequestContext Build()
        {
            return new ProfileDataRequestContext(
                new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(JwtClaimTypes.Subject, _subjectId) })), 
                _client, 
                _caller,
                _requestedClaimTypes);
        }
    }
}
