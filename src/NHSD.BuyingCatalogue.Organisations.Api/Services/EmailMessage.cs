using System;
using MimeKit;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    /// <summary>
    /// An e-mail message.
    /// </summary>
    internal sealed class EmailMessage
    {
        /// <summary>
        /// The placeholder to use in any body text that will be replaced by the URL
        /// for the password reset endpoint.
        /// </summary>
        internal const string ResetPasswordLinkPlaceholder = "[ResetPasswordLink]";

        private EmailAddress sender;
        private EmailAddress recipient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessage"/> class.
        /// </summary>
        /// <remarks>Required by <see cref="System.Text.Json"/>.</remarks>.
        public EmailMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessage"/> class based
        /// on the supplied <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to use as the basis of the newly constructed message.</param>
        /// <param name="passwordResetUrl">The password reset endpoint.</param>
        /// <remarks>This constructor will initialize a new <see cref="EmailMessage"/> using
        /// the sender, subject and bodies of the <paramref name="message"/> parameter.
        /// Any password reset placeholders (<see cref="ResetPasswordLinkPlaceholder"/> will
        /// be replaced by the value of <paramref name="passwordResetUrl"/>.</remarks>
        internal EmailMessage(EmailMessage message, Uri passwordResetUrl)
        {
            Sender = message.Sender;
            Subject = message.Subject;

            var passwordResetLink = passwordResetUrl.ToString();

            HtmlBody = message.HtmlBody.Replace(ResetPasswordLinkPlaceholder, passwordResetLink);
            TextBody = message.TextBody.Replace(ResetPasswordLinkPlaceholder, passwordResetLink);
        }

        /// <summary>
        /// Gets or sets the sender (from address) of the message.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langref="null"/>.</exception>
        public EmailAddress Sender
        {
            get => sender;
            set => sender = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML version of the body.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets or sets the plain text version of the body.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// Gets or sets the recipient (to address) of the message.
        /// </summary>
        /// /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langref="null"/>.</exception>
        internal EmailAddress Recipient
        {
            get => recipient;
            set => recipient = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Returns the receiver as a <see cref="MimeMessage"/>.
        /// </summary>
        /// <returns>the corresponding <see cref="MimeMessage"/>.</returns>
        internal MimeMessage AsMimeMessage()
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = HtmlBody,
                TextBody = TextBody,
            };

            var message = new MimeMessage
            {
                Subject = Subject,
                Body = bodyBuilder.ToMessageBody(),
            };

            message.From.Add(Sender.AsMailboxAddress());
            message.To.Add(Recipient.AsMailboxAddress());

            return message;
        }
    }
}
