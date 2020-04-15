using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;

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
        /// <param name="user">The user to send the e-mail to.</param>
        /// <returns>An asynchronous task context.</returns>
        Task SendInitialEmailAsync(ApplicationUser user);
    }
}
