using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.EmailTemplates;

namespace Ezenity_Backend.Services.Common
{
    public interface IEmailTemplateService : IBaseService<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest, DeleteResponse>
    {
    }
}
