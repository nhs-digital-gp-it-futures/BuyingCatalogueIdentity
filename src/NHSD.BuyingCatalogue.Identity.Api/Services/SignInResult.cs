namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    public sealed class SignInResult
    {
        internal SignInResult(bool isSuccessful, bool isTrustedReturnUrl = false, string loginHint = null)
        {
            IsTrustedReturnUrl = isTrustedReturnUrl;
            IsSuccessful = isSuccessful;
            LoginHint = loginHint;
        }

        internal bool IsSuccessful { get; }

        internal bool IsTrustedReturnUrl { get; }

        internal string LoginHint { get; }
    }
}
