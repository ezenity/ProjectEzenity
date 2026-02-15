using Ezenity.Core.Entities.Emails;
using System.Threading.Tasks;

namespace Ezenity.Application.Abstractions.Emails;

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
