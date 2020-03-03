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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        private readonly IEventService _eventService;

        public LogoutService(
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerInteractionService identityServerInteractionService,
            IEventService eventService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _identityServerInteractionService = identityServerInteractionService ?? throw new ArgumentNullException(nameof(identityServerInteractionService));
            _eventService = eventService;
        }

        public async Task<LogoutRequest> GetLogoutRequestAsync(string logoutId)
        {
            return await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
        }

        public async Task SignOutAsync(LogoutRequest logoutRequest)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            await httpContext.SignOutAsync();
            await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            if (logoutRequest is object)
            {
                await _eventService.RaiseAsync(new UserLogoutSuccessEvent(logoutRequest.SubjectId, logoutRequest.ClientName));
            }
        }
    }
}
