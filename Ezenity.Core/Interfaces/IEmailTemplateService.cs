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
        /// Gets an entity by its name.
        /// </summary>
        Task<EmailTemplateResponse> GetByNameAsync(string templateName);

        /// <summary>
        /// Renders Email Content using Razor Views.
        /// </summary>
        Task<string> RenderEmailTemplateAsync(string templateName, Dictionary<string, string> model);
    }
}
