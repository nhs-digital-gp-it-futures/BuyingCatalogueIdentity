using System;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    public sealed class LogoutRequestBuilder
    {
        private Uri _iframeUrl;
        private string _clientId;
        private string _clientName;
        private string _subjectId;
        private Uri _postLogoutRedirectUri;

        public LogoutRequestBuilder()
        {
            _iframeUrl = null;
            _clientId = "Bob@test.com";
            _clientName = "Bob";
            _subjectId = "1234";
            _postLogoutRedirectUri = new Uri("http://localhost");
        }

        public static LogoutRequestBuilder Create()
        {
            return new LogoutRequestBuilder();
        }

        public LogoutRequestBuilder WithIframeUrl(Uri iframeUrl)
        {
            _iframeUrl = iframeUrl;
            return this;
        }

        public LogoutRequestBuilder WithClientId(string clientId)
        {
            _clientId = clientId;
            return this;
        }

        public LogoutRequestBuilder WithClientName(string clientName)
        {
            _clientName = clientName;
            return this;
        }
        
        public LogoutRequestBuilder WithSubjectId(string subjectId)
        {
            _subjectId = subjectId;
            return this;
        }

        public LogoutRequestBuilder WithPostLogoutRedirectUri(Uri postLogoutRedirectUri)
        {
            _postLogoutRedirectUri = postLogoutRedirectUri;
            return this;
        }

        public LogoutRequest Build()
        {
            return new LogoutRequest(_iframeUrl?.ToString(), new LogoutMessage
            {
                ClientId = _clientId,
                ClientName = _clientName,
                SubjectId = _subjectId,
                PostLogoutRedirectUri = _postLogoutRedirectUri?.ToString()
            });
        }
    }
}
