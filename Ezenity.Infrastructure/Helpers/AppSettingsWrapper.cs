using Ezenity.Application.Abstractions.Configuration;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Wraps the IAppSettings interface to provide an abstraction layer over the application settings. 
    /// This allows for extending the functionality of app settings retrieval and facilitates easier unit testing.
    /// </summary>
    public class AppSettingsWrapper : IAppSettings
    {
        private readonly IAppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the AppSettingsWrapper class with the specified IAppSettings implementation.
        /// </summary>
        /// <param name="appSettings">The actual implementation of IAppSettings to wrap.</param>
        /// <exception cref="ArgumentException">Thrown if the provided appSettings is null.</exception>
        public AppSettingsWrapper(IAppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Gets the base URL of the application.
        /// </summary>
        public string BaseUrl => _appSettings.BaseUrl;

        /// <summary>
        /// Gets the secret key used for cryptographic operations.
        /// </summary>
        public string Secret => _appSettings.Secret;

        /// <summary>
        /// Gets the Time-To-Live (TTL) for refresh tokens, in days.
        /// </summary>
        public int RefreshTokenTTL => _appSettings.RefreshTokenTTL;

        /// <summary>
        /// Gets the email address that will be used as the 'From' address in outgoing emails.
        /// </summary>
        public string EmailFrom => _appSettings.EmailFrom;

        /// <summary>
        /// Gets the host name for the SMTP server used for sending emails.
        /// </summary>
        public string SmtpHost => _appSettings.SmtpHost;

        /// <summary>
        /// Gets the port number for the SMTP server.
        /// </summary>
        public int SmtpPort => _appSettings.SmtpPort;

        /// <summary>
        /// Gets the username for authentication with the SMTP server.
        /// </summary>
        public string SmtpUser => _appSettings.SmtpUser;

        /// <summary>
        /// Gets the password for authentication with the SMTP server.
        /// </summary>
        public string SmtpPass => _appSettings.SmtpPass;

        /// <summary>
        /// Gets a value indicating whether SSL is enabled for SMTP communications.
        /// </summary>
        public bool SmtpEnabledSsl => _appSettings.SmtpEnabledSsl;

        /// <summary>
        /// Gets the access token used for some third-party services or internal processes.
        /// </summary>
        public string AccessToken => _appSettings.AccessToken;

        /// <summary>
        /// Gets the email domain that has been set for the message id in an email.
        /// </summary>
        public string EmailMessageIdDomain => _appSettings.EmailMessageIdDomain;

        /// <summary>
        /// Gets the base path for an Razor Email Template
        /// </summary>
        public string EmailTemplateBasePath => _appSettings.EmailTemplateBasePath;
  }
}
