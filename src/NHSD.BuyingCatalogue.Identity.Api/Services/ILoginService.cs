using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Models.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface ILoginService
    {
        Task<Result<SignInResult>> SignInAsync(string username, string password, Uri returnUrl);
    }
}
