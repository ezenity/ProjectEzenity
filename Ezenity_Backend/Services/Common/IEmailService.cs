using Ezenity_Backend.Entities.Emails;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
