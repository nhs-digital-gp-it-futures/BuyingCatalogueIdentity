using System;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface ILoginService
    {
        Task<SignInResult> SignInAsync(string username, string password, Uri returnUrl);
    }
}
