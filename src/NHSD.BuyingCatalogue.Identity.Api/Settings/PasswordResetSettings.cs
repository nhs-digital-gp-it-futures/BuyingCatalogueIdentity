using NHSD.BuyingCatalogue.Identity.Common.Email;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    /// <summary>
    /// Password reset settings.
    /// </summary>
    internal sealed class PasswordResetSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of
        /// the password reset e-mail message.
        /// </summary>
        public EmailMessage EmailMessage { get; set; }
    }
}
