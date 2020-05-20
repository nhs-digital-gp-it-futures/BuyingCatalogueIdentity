using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    /// <summary>
    /// Defines operations for registering a new user.
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Asynchronously sends the initial e-mail to a user following registration.
        /// </summary>
        /// <param name="token">The reset token details.</param>
        /// <returns>An asynchronous task context.</returns>
        Task SendInitialEmailAsync(PasswordResetToken token);
    }
}
