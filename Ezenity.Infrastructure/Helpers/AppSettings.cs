using Ezenity.Core.Interfaces;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Contains settings for the application.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        public AppSettings(string baseUrl, string secret, int refreshTokenTTL, string emailFrom, string smtpHost, int smtpPort, string smtpUser, string smtpPass, bool smtpEnableSsl, string accessToken)
        {
            BaseUrl = baseUrl;
            Secret = secret;
            RefreshTokenTTL = refreshTokenTTL;
            EmailFrom = emailFrom;
            SmtpHost = smtpHost;
            SmtpPort = smtpPort;
            SmtpUser = smtpUser;
            SmtpPass = smtpPass;
            SmtpEnableSsl = smtpEnableSsl;
            AccessToken = accessToken;
        }

        /// <summary>
        /// Gets the base URL of the application.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// Gets the secret used for cryptographic operations.
        /// </summary>
        public string Secret { get; }

        /// <summary>
        /// Gets the Time-To-Live (TTL) for refresh tokens (in days).
        /// </summary>
        public int RefreshTokenTTL { get; }

        /// <summary>
        /// Gets the email address that will be used as the 'From' address in outgoing emails.
        /// </summary>
        public string EmailFrom { get; }

        /// <summary>
        /// Gets the host name for the SMTP server used for sending emails.
        /// </summary>
        public string SmtpHost { get; }

        /// <summary>
        /// Gets the port number for the SMTP server.
        /// </summary>
        public int SmtpPort { get; }

        /// <summary>
        /// Gets the username for authentication with the SMTP server.
        /// </summary>
        public string SmtpUser { get; }

        /// <summary>
        /// Gets the password for authentication with the SMTP server.
        /// </summary>
        public string SmtpPass { get; }

        /// <summary>
        /// Gets a value indicating whether SSL is enabled for SMTP communications.
        /// </summary>
        public bool SmtpEnableSsl { get; }

        /// <summary>
        /// Gets the access token used for some third-party services or internal processes.
        /// </summary>
        public string AccessToken { get; }
    }
}
