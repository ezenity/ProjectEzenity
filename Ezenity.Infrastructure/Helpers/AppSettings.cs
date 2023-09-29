namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Contains settings for the application.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the base URL of the application.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the secret used for cryptographic operations.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the Time-To-Live (TTL) for refresh tokens (in days).
        /// </summary>
        public int RefreshTokenTTL { get; set; }

        /// <summary>
        /// Gets or sets the email address that will be used as the 'From' address in outgoing emails.
        /// </summary>
        public string EmailFrom { get; set; }

        /// <summary>
        /// Gets or sets the host name for the SMTP server used for sending emails.
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// Gets or sets the port number for the SMTP server.
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Gets or sets the username for authentication with the SMTP server.
        /// </summary>
        public string SmtpUser { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication with the SMTP server.
        /// </summary>
        public string SmtpPass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled for SMTP communications.
        /// </summary>
        public bool SmtpEnableSsl { get; set; }

        /// <summary>
        /// Gets or sets the access token used for some third-party services or internal processes.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
