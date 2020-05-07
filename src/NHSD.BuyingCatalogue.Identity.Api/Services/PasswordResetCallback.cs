using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHSD.BuyingCatalogue.Identity.Api.Controllers;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Common.Extensions;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    internal sealed class PasswordResetCallback : IPasswordResetCallback
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly IssuerSettings _issuerSettings;

        public PasswordResetCallback(IHttpContextAccessor accessor, LinkGenerator generator, IssuerSettings issuerSettings)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _issuerSettings = issuerSettings ?? throw new ArgumentNullException(nameof(issuerSettings));
        }

        public Uri GetPasswordResetCallback(PasswordResetToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            var context = _accessor.HttpContext;
            
            var action = _generator.GetUriByAction(
                context,
                nameof(AccountController.ResetPassword), 
                nameof(AccountController).TrimController(),
                new { token.Token, token.User.Email },
                context.Request.Scheme,
                HostString.FromUriComponent(_issuerSettings.IssuerUrl)
                );

            return new Uri(action);
        }
    }
}
