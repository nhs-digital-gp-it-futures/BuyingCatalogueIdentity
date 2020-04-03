using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    /// <summary>
    /// Password services.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Generates a password reset token for the user with the
        /// provided <paramref name="emailAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The e-mail of the user to generate the password
        /// reset token for.</param>
        /// <returns>A <see cref="PasswordResetToken"/> if the user was found;
        /// otherwise, <see lagref="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="emailAddress"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="emailAddress"/> is empty or white space.</exception>
        Task<PasswordResetToken> GeneratePasswordResetTokenAsync(string emailAddress);

        /// <summary>
        /// Sends a password reset e-mail to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to send the e-mail to.</param>
        /// <param name="callback">The callback URL to handle
        /// the password reset.</param>
        /// <returns>An asynchronous task context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is <see langref="null"/>.</exception>
        Task SendResetEmailAsync(ApplicationUser user, string callback);
    }
}
