using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.EmailTemplates;

namespace Ezenity_Backend.Services.Common
{
    /// <summary>
    /// Service for managing email templates.
    /// </summary>
    public interface IEmailTemplateService : IBaseService<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest, DeleteResponse>
    {
    }
}
