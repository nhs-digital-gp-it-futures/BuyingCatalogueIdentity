using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface ILoginService
    {
        Task<Result<SignInResponse>> SignInAsync(string username, string password, Uri returnUrl);
    }
}
