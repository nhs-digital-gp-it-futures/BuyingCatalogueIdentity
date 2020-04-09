using NHSD.BuyingCatalogue.Identity.Common.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Errors
{
    public static class LoginUserErrors
    {
        public static ErrorMessage UserNameOrPasswordIncorrect()
        {
            return new ErrorMessage("UsernameOrPasswordIsIncorrect");
        }

        public static ErrorMessage UserIsDisabled()
        {
            return new ErrorMessage("UserIsDisabled");
        }
    }
}
