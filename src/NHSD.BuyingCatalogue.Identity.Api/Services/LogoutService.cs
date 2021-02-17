using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class LogoutService : ILogoutService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IIdentityServerInteractionService identityServerInteractionService;
        private readonly IEventService eventService;

        public LogoutService(
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerInteractionService identityServerInteractionService,
            IEventService eventService)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.identityServerInteractionService = identityServerInteractionService ?? throw new ArgumentNullException(nameof(identityServerInteractionService));
            this.eventService = eventService;
        }

        public async Task<LogoutRequest> GetLogoutRequestAsync(string logoutId)
        {
            return await identityServerInteractionService.GetLogoutContextAsync(logoutId);
        }

        public async Task SignOutAsync(LogoutRequest logoutRequest)
        {
            HttpContext httpContext = httpContextAccessor.HttpContext;

            await httpContext!.SignOutAsync();
            await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            if (logoutRequest is not null)
            {
                await eventService.RaiseAsync(new UserLogoutSuccessEvent(logoutRequest.SubjectId, logoutRequest.ClientName));
            }
        }
    }
}
