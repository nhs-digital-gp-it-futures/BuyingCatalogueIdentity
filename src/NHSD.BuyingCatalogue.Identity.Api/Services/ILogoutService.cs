using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.ViewModels.Account;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface ILogoutService
    {
        Task<string> GetPostLogoutRedirectUri(string logoutId);

        Task SignOutAsync(LogoutViewModel logoutViewModel);
    }
}
