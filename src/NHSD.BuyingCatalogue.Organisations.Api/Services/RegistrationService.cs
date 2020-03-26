using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    /// <summary>
    /// Provides registration-related services.
    /// </summary>
    internal sealed class RegistrationService : IRegistrationService
    {
        private readonly IEmailService _emailService;
        private readonly RegistrationSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationService"/> class using
        /// the provided <paramref name="emailService"/> and <paramref name="settings"/>.
        /// </summary>
        /// <param name="emailService">The service to use to send e-mails.</param>
        /// <param name="settings">The configured registration settings.</param>
        public RegistrationService(IEmailService emailService, RegistrationSettings settings)
        {
            _emailService = emailService;
            _settings = settings;
        }

        /// <summary>
        /// Asynchronously sends the initial e-mail to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to send the e-mail to.</param>
        /// <returns>An asynchronous task context.</returns>
        public async Task SendInitialEmailAsync(ApplicationUser user)
        {
            // TODO: replace hard-coded Uri with reset password endpoint once available
            var message = new EmailMessage(_settings.EmailMessage, new Uri("https://www.google.co.uk/"))
            {
                Recipient = new EmailAddress(user.DisplayName, user.Email),
            };

            await _emailService.SendEmailAsync(message);
        }
    }
}
