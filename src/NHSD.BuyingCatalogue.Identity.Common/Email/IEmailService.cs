using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Identity.Common.Email
{
    /// <summary>
    /// Defines operations for sending e-mails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an e-mail message asynchronously.
        /// </summary>
        /// <param name="emailMessage">The e-mail message to send asynchronously.</param>
        /// <returns>An asynchronous task context.</returns>
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
