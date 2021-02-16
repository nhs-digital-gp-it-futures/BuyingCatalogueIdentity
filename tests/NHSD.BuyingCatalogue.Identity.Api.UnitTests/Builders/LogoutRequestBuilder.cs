using System;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class LogoutRequestBuilder
    {
        private readonly Uri iFrameUrl;
        private readonly string clientId;
        private readonly string clientName;
        private readonly string subjectId;
        private readonly Uri postLogoutRedirectUri;

        public LogoutRequestBuilder()
        {
            iFrameUrl = null;
            clientId = "Bob@test.com";
            clientName = "Bob";
            subjectId = "1234";
            postLogoutRedirectUri = new Uri("http://localhost");
        }

        public static LogoutRequestBuilder Create()
        {
            return new();
        }

        public LogoutRequest Build()
        {
            return new(iFrameUrl?.ToString(), new LogoutMessage
            {
                ClientId = clientId,
                ClientName = clientName,
                SubjectId = subjectId,
                PostLogoutRedirectUri = postLogoutRedirectUri?.ToString(),
            });
        }
    }
}
