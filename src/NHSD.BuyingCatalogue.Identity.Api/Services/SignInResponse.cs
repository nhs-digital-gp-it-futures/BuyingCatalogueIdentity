namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class SignInResponse
    {
        internal SignInResponse(bool isTrustedReturnUrl = false, string loginHint = null)
        {
            IsTrustedReturnUrl = isTrustedReturnUrl;
            LoginHint = loginHint;
        }

        internal bool IsTrustedReturnUrl { get; }

        internal string LoginHint { get; }
    }
}
