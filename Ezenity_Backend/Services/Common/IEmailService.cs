using Ezenity_Backend.Entities;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
{
    public interface IEmailService
    {
        Task SendEmailAsync(IEmailMessage message);
    }
}
