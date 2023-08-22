using Ezenity_Backend.Services.Emails;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
