using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.EmailTemplates;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.Common
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
