using NHSD.BuyingCatalogue.Identity.Common.Email;

namespace NHSD.BuyingCatalogue.Organisations.Api.Settings
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
        public EmailMessage EmailMessage { get; set; }
    }
}
