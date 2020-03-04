using System.Threading.Tasks;
using IdentityServer4.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface ILogoutService
    {
        Task<LogoutRequest> GetLogoutRequestAsync(string logoutId);

        Task SignOutAsync(LogoutRequest logoutViewModel);
    }
}
