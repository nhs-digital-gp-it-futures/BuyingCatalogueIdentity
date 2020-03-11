using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByIdAsync(string userId);
    }
}
