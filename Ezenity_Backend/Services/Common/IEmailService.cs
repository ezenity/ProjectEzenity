using Ezenity_Backend.Entities.Emails;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
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
