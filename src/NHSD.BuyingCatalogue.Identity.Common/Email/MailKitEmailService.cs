﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MailKit;
using NHSD.BuyingCatalogue.Identity.Common.Settings;

namespace NHSD.BuyingCatalogue.Identity.Common.Email
{
    /// <summary>
    /// A service for sending e-mails using MailKit.
    /// </summary>
    public sealed class MailKitEmailService : IEmailService
    {
        private readonly IMailTransport _client;
        private readonly SmtpSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailService"/> class
        /// using the provided <paramref name="client"/> and <paramref name="settings"/>.
        /// </summary>
        /// <param name="client">The mail transport to use to send e-mail.</param>
        /// <param name="settings">The SMTP configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langref="null"/>.</exception>
        [SuppressMessage(
            "Security",
            "CA5359:Do Not Disable Certificate Validation",
            Justification = "Certificate validation only disabled when specified in configuration (for use in test environments only)")]
        public MailKitEmailService(IMailTransport client, SmtpSettings settings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settings = settings ?? throw new ArgumentNullException(nameof(client));

            if (settings.AllowInvalidCertificate.GetValueOrDefault())
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
        }

        /// <summary>
        /// Sends an e-mail message asynchronously using the
        /// provided <see cref="IMailTransport"/> instance.
        /// </summary>
        /// <param name="emailMessage">The e-mail message to send asynchronously.</param>
        /// <returns>An asynchronous task context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="emailMessage"/> is <see langref="null"/>.</exception>
        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            if (emailMessage is null)
                throw new ArgumentNullException(nameof(emailMessage));

            await _client.ConnectAsync(_settings.Host, _settings.Port);

            try
            {
                var authentication = _settings.Authentication;
                if (authentication.IsRequired)
                    await _client.AuthenticateAsync(authentication.UserName, authentication.Password);
                
                await _client.SendAsync(emailMessage.AsMimeMessage(_settings.EmailSubjectPrefix));
            }
            finally
            {
                if (_client.IsConnected)
                    await _client.DisconnectAsync(true);
            }
        }
    }
}
