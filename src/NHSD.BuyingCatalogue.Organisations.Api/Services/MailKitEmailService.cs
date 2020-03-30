using System.Threading.Tasks;
using MailKit;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    /// <summary>
    /// A service for sending e-mails using MailKit.
    /// </summary>
    internal sealed class MailKitEmailService : IEmailService
    {
        private readonly IMailTransport _client;
        private readonly SmtpSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailService"/> class
        /// using the provided <paramref name="client"/> and <paramref name="settings"/>.
        /// </summary>
        /// <param name="client">The mail transport to use to send e-mail.</param>
        /// <param name="settings">The SMTP configuration.</param>
        public MailKitEmailService(IMailTransport client, SmtpSettings settings)
        {
            _client = client;

            if (settings.AllowInvalidCertificate.GetValueOrDefault())
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            _settings = settings;
        }

        /// <summary>
        /// Sends an e-mail message asynchronously using the
        /// provided <see cref="IMailTransport"/> instance.
        /// </summary>
        /// <param name="emailMessage">The e-mail message to send asynchronously.</param>
        /// <returns>An asynchronous task context.</returns>
        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            await _client.ConnectAsync(_settings.Host, _settings.Port);

            var authentication = _settings.Authentication;
            if (authentication.IsRequired)
                await _client.AuthenticateAsync(authentication.UserName, authentication.Password);

            await _client.SendAsync(emailMessage.AsMimeMessage());
            await _client.DisconnectAsync(true);
        }
    }
}
