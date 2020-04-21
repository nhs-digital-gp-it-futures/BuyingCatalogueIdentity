using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Common.Results;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public interface IAgreementConsentService
    {
        Task<bool> IsValidReturnUrl(Uri returnUrl);

        Task<Result<Uri>> GrantConsent(Uri returnUrl, string subjectId);
    }
}
