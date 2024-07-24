using Ezenity.Core.Interfaces;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Represents the application-specific settings used throughout the application. 
    /// Holds configuration details like base URL, SMTP settings, tokens, and other critical settings.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// Initializes a new instance of the AppSettings class with the specified settings values.
        /// </summary>
        /// <param name="baseUrl">The base URL of the application.</param>
        /// <param name="secret">The secret key used for cryptographic operations.</param>
        /// <param name="refreshTokenTTL">The Time-To-Live (TTL) for refresh tokens, in days.</param>
        /// <param name="emailFrom">The email address that will be used as the 'From' address in outgoing emails.</param>
        /// <param name="smtpHost">The host name for the SMTP server used for sending emails.</param>
        /// <param name="smtpPort">The port number for the SMTP server.</param>
        /// <param name="smtpUser">The username for authentication with the SMTP server.</param>
        /// <param name="smtpPass">The password for authentication with the SMTP server.</param>
        /// <param name="smtpEnableSsl">A value indicating whether SSL is enabled for SMTP communications.</param>
        /// <param name="accessToken">The access token used for some third-party services or internal processes.</param>
        /// <param name="emailMessageIdDomain">The email domain that is being used for a message id in an email.</param>
        public AppSettings(string baseUrl, string secret, int refreshTokenTTL, string emailFrom, string smtpHost, int smtpPort, string smtpUser, string smtpPass, bool smtpEnableSsl, string accessToken, string emailMessageIdDomain, string emailTemplateBasePath)
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
            EmailMessageIdDomain = emailMessageIdDomain;
            EmailTemplateBasePath = emailTemplateBasePath;
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

        /// <summary>
        /// Gets the email domain that has been set for the message id in an email.
        /// </summary>
        public string EmailMessageIdDomain { get; }

        /// <summary>
        /// Gets the base path for an Razor Email Template
        /// </summary>
        public string EmailTemplateBasePath { get; }
  }
}
