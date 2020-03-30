using System;
using MimeKit;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    /// <summary>
    /// An e-mail address.
    /// </summary>
    internal sealed class EmailAddress
    {
        private string address;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddress"/> class.
        /// </summary>
        /// <remarks>Required by <see cref="System.Text.Json"/>.</remarks>
        public EmailAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddress"/> class
        /// with the given name and address.
        /// </summary>
        /// <param name="name">An optional </param>
        /// <param name="address"></param>
        internal EmailAddress(string name, string address)
        {
            Name = name;
            Address = address;
        }

        /// <summary>
        /// Gets or sets the display name of the address.
        /// </summary>
        /// <remarks>An optional display name, for example
        /// Buying Catalogue Team.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the actual e-mail address.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is empty or white space.</exception>
        public string Address
        {
            get => address;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException($"{nameof(value)} cannot be empty or white space.", nameof(value));

                address = value;
            }
        }

        /// <summary>
        /// Returns the receiver as a <see cref="MailboxAddress"/>.
        /// </summary>
        /// <returns>the corresponding <see cref="MailboxAddress"/>.</returns>
        internal MailboxAddress AsMailboxAddress() => new MailboxAddress(Name, Address);
    }
}
