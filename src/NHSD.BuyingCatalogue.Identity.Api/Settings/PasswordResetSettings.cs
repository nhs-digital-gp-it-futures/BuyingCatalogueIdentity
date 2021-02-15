using NHSD.BuyingCatalogue.EmailClient;

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
        public EmailMessageTemplate EmailMessageTemplate { get; set; }
    }
}
