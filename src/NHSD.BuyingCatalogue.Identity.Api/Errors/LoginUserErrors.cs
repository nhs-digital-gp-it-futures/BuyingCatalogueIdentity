using NHSD.BuyingCatalogue.Identity.Common.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Errors
{
    internal static class LoginUserErrors
    {
        internal static ErrorDetails UserNameOrPasswordIncorrect()
        {
            return new("UsernameOrPasswordIsIncorrect");
        }

        internal static ErrorDetails UserIsDisabled()
        {
            return new("UserIsDisabled");
        }
    }
}
