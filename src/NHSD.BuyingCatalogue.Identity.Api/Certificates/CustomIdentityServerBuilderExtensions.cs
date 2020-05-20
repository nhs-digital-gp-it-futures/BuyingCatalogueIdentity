using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace NHSD.BuyingCatalogue.Identity.Api.Certificates
{
    internal static class CustomIdentityServerBuilderExtensions
    {
        internal static IIdentityServerBuilder AddCustomSigningCredential(
            this IIdentityServerBuilder builder,
            ICertificate certificate,
            ILogger logger)
        {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));

            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            if (!certificate.IsAvailable)
            {
                logger.Information("Using developer signing credential");
                return builder.AddDeveloperSigningCredential();
            }

            logger.Information("Using certificate {path}", certificate.Path);

            return builder.AddSigningCredential(certificate.Instance);
        }
    }
}
