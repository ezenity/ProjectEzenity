using Ezenity.Core.Entities.Emails;

namespace Ezenity.Core.Services.Common
{
    /// <summary>
    /// Service for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email based on the provided EmailMessage.
        /// </summary>
        Task SendEmailAsync(EmailMessage message);
    }
}
