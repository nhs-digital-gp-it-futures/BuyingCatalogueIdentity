using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Infrastructure;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class LogoutService : ILogoutService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public LogoutService(
            IHttpContextAccessor httpContextAccessor,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _identityServerInteractionService = identityServerInteractionService ?? throw new ArgumentNullException(nameof(identityServerInteractionService));
        }

        public async Task<string> GetPostLogoutRedirectUri(string logoutId)
        {
            LogoutRequest logoutRequest = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
            return logoutRequest?.PostLogoutRedirectUri;
        }

        public async Task SignOutAsync(LogoutViewModel logoutViewModel)
        {
            logoutViewModel.ThrowIfNull(nameof(logoutViewModel));

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            await httpContext.SignOutAsync();
            await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
