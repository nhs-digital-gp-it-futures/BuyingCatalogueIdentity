using NHSD.BuyingCatalogue.EmailClient;

namespace NHSD.BuyingCatalogue.Identity.Api.Settings
{
    /// <summary>
    /// Registration settings.
    /// </summary>
    internal sealed class RegistrationSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of
        /// the e-mail message to send to a new user.
        /// </summary>
        public EmailMessageTemplate EmailMessage { get; set; }
    }
}
