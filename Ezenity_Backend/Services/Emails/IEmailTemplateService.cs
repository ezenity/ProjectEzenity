using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Models.Emails;

namespace Ezenity_Backend.Services.Emails
{
    public interface IEmailTemplateService : IBaseService<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest>
    {
    }
}
