namespace NHSD.BuyingCatalogue.Organisations.Api.Settings
{
    /// <summary>
    /// SMTP server settings.
    /// </summary>
    internal sealed class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the authentication settings for the SMTP server.
        /// </summary>
        public SmtpAuthenticationSettings Authentication { get; set; }

        /// <summary>
        /// Gets or sets the host name of the SMTP server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port to use to connect to the SMTP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is required
        /// when connecting to the SMTP server.
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
