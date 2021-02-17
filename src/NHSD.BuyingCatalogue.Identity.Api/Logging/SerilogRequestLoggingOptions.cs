using System;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace NHSD.BuyingCatalogue.Identity.Api.Logging
{
    public static class SerilogRequestLoggingOptions
    {
        internal const string HealthCheckEndpointDisplayName = "Health checks";

        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (diagnosticContext is null)
            {
                throw new ArgumentNullException(nameof(diagnosticContext));
            }

            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a query string right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is not null)
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        public static LogEventLevel GetLevel(HttpContext httpContext, double elapsed, Exception exception)
        {
            _ = elapsed;

            if (exception is not null)
                return LogEventLevel.Error;

            if (httpContext is null || httpContext.Response.StatusCode > 499)
                return LogEventLevel.Error;

            return IsHealthCheck(httpContext)
                ? LogEventLevel.Verbose
                : LogEventLevel.Information;
        }

        private static bool IsHealthCheck(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();

            return endpoint is not null && string.Equals(
                endpoint.DisplayName,
                HealthCheckEndpointDisplayName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
