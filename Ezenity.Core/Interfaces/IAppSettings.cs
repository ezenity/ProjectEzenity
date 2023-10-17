using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Core.Interfaces
{
    /// <summary>
    /// Contains settings for the application.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the base URL of the application.
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Gets the secret used for cryptographic operations.
        /// </summary>
        string Secret { get; }

        /// <summary>
        /// Gets the Time-To-Live (TTL) for refresh tokens (in days).
        /// </summary>
        int RefreshTokenTTL { get; }

        /// <summary>
        /// Gets the email address that will be used as the 'From' address in outgoing emails.
        /// </summary>
        string EmailFrom { get; }

        /// <summary>
        /// Gets the host name for the SMTP server used for sending emails.
        /// </summary>
        string SmtpHost { get; }

        /// <summary>
        /// Gets the port number for the SMTP server.
        /// </summary>
        int SmtpPort { get; }

        /// <summary>
        /// Gets the username for authentication with the SMTP server.
        /// </summary>
        string SmtpUser { get; }

        /// <summary>
        /// Gets the password for authentication with the SMTP server.
        /// </summary>
        string SmtpPass { get; }

        /// <summary>
        /// Gets a value indicating whether SSL is enabled for SMTP communications.
        /// </summary>
        bool SmtpEnableSsl { get; }

        /// <summary>
        /// Gets the access token used for some third-party services or internal processes.
        /// </summary>
        string AccessToken { get; }
    }
}
