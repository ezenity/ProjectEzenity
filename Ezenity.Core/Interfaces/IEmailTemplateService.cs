using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.EmailTemplates;

namespace Ezenity.Core.Services.Common
{
    /// <summary>
    /// Service for managing email templates.
    /// </summary>
    public interface IEmailTemplateService : IBaseService<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest, DeleteResponse>
    {
        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        Task<EmailTemplateNonDynamicResponse> GetNonDynamicByIdAsync(int id);
    }
}
