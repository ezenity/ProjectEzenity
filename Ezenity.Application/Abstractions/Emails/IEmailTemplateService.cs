using Ezenity.Contracts;
using Ezenity.Contracts.Models.EmailTemplates;
using Ezenity.Domain.Entities.EmailTemplates;

namespace Ezenity.Application.Abstractions.Emails;

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
