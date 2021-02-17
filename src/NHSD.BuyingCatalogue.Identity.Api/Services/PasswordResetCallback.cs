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
        private readonly IHttpContextAccessor accessor;
        private readonly LinkGenerator generator;
        private readonly IssuerSettings issuerSettings;

        public PasswordResetCallback(IHttpContextAccessor accessor, LinkGenerator generator, IssuerSettings issuerSettings)
        {
            this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
            this.issuerSettings = issuerSettings ?? throw new ArgumentNullException(nameof(issuerSettings));
        }

        public Uri GetPasswordResetCallback(PasswordResetToken token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var context = accessor.HttpContext;
            var hostString = new HostString(issuerSettings.IssuerUrl.Authority);

            var action = generator.GetUriByAction(
                context,
                nameof(AccountController.ResetPassword),
                nameof(AccountController).TrimController(),
                new { token.Token, token.User.Email },
                issuerSettings.IssuerUrl.Scheme,
                hostString);

            return new Uri(action);
        }
    }
}
