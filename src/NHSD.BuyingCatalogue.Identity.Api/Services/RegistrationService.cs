using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.EmailClient;
using NHSD.BuyingCatalogue.Identity.Api.Extensions;
using NHSD.BuyingCatalogue.Identity.Api.Settings;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    /// <summary>
    /// Provides registration-related services.
    /// </summary>
    internal sealed class RegistrationService : IRegistrationService
    {
        private readonly IEmailService emailService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly RegistrationSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationService"/> class using
        /// the provided <paramref name="emailService"/> and <paramref name="settings"/>.
        /// </summary>
        /// <param name="emailService">The service to use to send e-mails.</param>
        /// <param name="passwordResetCallback">The instance that will supply the reset callback URL.</param>
        /// <param name="settings">The configured registration settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="emailService"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="passwordResetCallback"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langref="null"/>.</exception>
        public RegistrationService(IEmailService emailService, IPasswordResetCallback passwordResetCallback, RegistrationSettings settings)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Asynchronously sends the initial e-mail using the provided <paramref name="token"/> details.
        /// </summary>
        /// <param name="token">The reset token information.</param>
        /// <returns>An asynchronous task context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="token"/> is <see langref="null"/>.</exception>
        public async Task SendInitialEmailAsync(PasswordResetToken token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var user = token.User;

            await emailService.SendEmailAsync(
                settings.EmailMessage,
                new EmailAddress(user.Email, user.DisplayName),
                passwordResetCallback.GetPasswordResetCallback(token));
        }
    }
}
