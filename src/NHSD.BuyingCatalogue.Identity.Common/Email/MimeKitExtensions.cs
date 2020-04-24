using MimeKit;

namespace NHSD.BuyingCatalogue.Identity.Common.Email
{
    public static class MimeKitExtensions
    {
        /// <summary>
        /// Returns the receiver as a <see cref="MailboxAddress"/>.
        /// </summary>
        /// <param name="address">The receiving <see cref="EmailAddress"/> instance.</param>
        /// <returns>the corresponding <see cref="MailboxAddress"/>.</returns>
        internal static MailboxAddress AsMailboxAddress(this EmailAddress address)
            => new MailboxAddress(address.DisplayName, address.Address);

        /// <summary>
        /// Returns the receiver as a <see cref="MimeMessage"/>.
        /// </summary>
        /// <param name="emailMessage">The receiving <see cref="EmailMessage"/> instance.</param>
        /// <param name="emailSubjectPrefix"></param>
        /// <returns>the corresponding <see cref="MimeMessage"/>.</returns>
        internal static MimeMessage AsMimeMessage(this EmailMessage emailMessage, string? emailSubjectPrefix)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailMessage.HtmlBody,
                TextBody = emailMessage.TextBody,
            };

            var message = new MimeMessage
            {
                Body = bodyBuilder.ToMessageBody(),
            };

            message.Subject = string.IsNullOrWhiteSpace(emailSubjectPrefix) && emailMessage.Subject != null
                ? emailMessage.Subject
                : $"{emailSubjectPrefix} {emailMessage.Subject}";

            message.From.Add(emailMessage.Sender?.AsMailboxAddress());
            message.To.Add(emailMessage.Recipient?.AsMailboxAddress());

            return message;
        }
    }
}
