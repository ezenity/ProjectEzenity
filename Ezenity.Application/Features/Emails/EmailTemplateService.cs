using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ezenity.Application.Abstractions.Configuration;
using Ezenity.Application.Abstractions.Emails;
using Ezenity.Application.Abstractions.Persistence;
using Ezenity.Application.Abstractions.Security;
using Ezenity.Application.Common.Exceptions;
using Ezenity.Application.Features.Auth;
using Ezenity.Contracts;
using Ezenity.Contracts.Models.EmailTemplates;
using Ezenity.Contracts.Models.Pages;
using Ezenity.Domain.Entities.EmailTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ezenity.Application.Features.Emails;

/// <summary>
/// Provides functionality to manage Email Templates.
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    /// <summary>
    /// Provides data access to the application's data store.
    /// </summary>
    private readonly IUnitOfWork _uow;
    private readonly IEmailTemplateRepository _emailTemplates;

    /// <summary>
    /// Provides object-object mapping functionality.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Provides access to application settings.
    /// </summary>
    private readonly IAppSettings _appSettings;

    /// <summary>
    /// Provides logging capabilities.
    /// </summary>
    private readonly ILogger<EmailTemplateService> _logger;

    /// <summary>
    /// Provides token generation and validation services.
    /// </summary>
    private readonly ITokenService _tokenHelper;

    /// <summary>
    /// Provides user authentication services.
    /// </summary>
    private readonly IAuthService _authService;

    /// <summary>
    /// Provides rendering for Razor views.
    /// </summary>
    //private readonly IRazorViewToStringRenderer _razorRenderer;
    private readonly IRazorViewRenderer _razorRenderer;

    /// <summary>
    /// Used to resolve template paths.
    /// </summary>
    private readonly IEmailTemplateResolver _emailTemplateResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
    /// </summary>
    /// <param name="context">Provides data access to the application's data store.</param>
    /// <param name="mapper">Provides object-object mapping functionality.</param>
    /// <param name="appSettings">Provides access to application settings.</param>
    /// <param name="logger">Provides logging capabilities.</param>
    /// <param name="tokenHelper">Provides token generation and validation services.</param>
    /// <param name="authService">Provides user authentication services.</param>
    /// <param name="razorRenderer">Provides rendering for Razor views.</param>
    /// <param name="emailTemplateResolver">Provides the resolved template paths.</param>
    public EmailTemplateService(IUnitOfWork uow, IEmailTemplateRepository emailTemplates, IMapper mapper, IAppSettings appSettings, ILogger<EmailTemplateService> logger, ITokenService tokenHelper, IAuthService authService, IRazorViewRenderer razorRenderer, IEmailTemplateResolver emailTemplateResolver)
    {
        _uow = uow ?? throw new ArgumentException(nameof(uow));
        _emailTemplates = emailTemplates ?? throw new ArgumentException(nameof(emailTemplates));
        _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
        _tokenHelper = tokenHelper ?? throw new ArgumentException(nameof(tokenHelper));
        _authService = authService ?? throw new ArgumentException(nameof(authService));
        _razorRenderer = razorRenderer ?? throw new ArgumentException(nameof(razorRenderer));
        _emailTemplateResolver = emailTemplateResolver ?? throw new ArgumentException(nameof(emailTemplateResolver));
    }

    /// <summary>
    /// Fetches an email template by its ID.
    /// </summary>
    /// <param name="id">The ID of the email template to fetch.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the EmailTemplateResponse.</returns>
    public async Task<EmailTemplateResponse> GetByIdAsync(int id)
    {
        //var emailTemplate = await GetEmailTemplate(id);
        //var emailTemplate = await _context.EmailTemplates
        //                        .Where(x => x.Id == id)
        //                        .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
        //                        .SingleOrDefaultAsync();

        var emailTemplate = await _emailTemplates.Query()
                                    .AsNoTracking()
                                    .Where(x => x.Id == id)
                                    .ProjectTo<EmailTemplateResponse>
                                        (_mapper.ConfigurationProvider)
                                    .SingleOrDefaultAsync();


        if (emailTemplate is null)
            throw new ResourceNotFoundException($"Email Template ID, {id}, was not found.");

        //return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        return emailTemplate;
    }

    /// <summary>
    /// Fetches an email template by its name.
    /// </summary>
    /// <param name="templateName">The name of the email template to fetch.</param>
    /// <returns>A task that represents the asynchronous opertion. The task result contains the EmailTemplateResponse.</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    public async Task<EmailTemplateResponse> GetByNameAsync(string templateName)
    {
        if (string.IsNullOrWhiteSpace(templateName))
            throw new AppException("Template name is required.");

        templateName = templateName.Trim();

        var emailTemplate = await _emailTemplates.Query()
                                    .AsNoTracking()
                                    .Where(n => n.TemplateName == templateName)
                                    .ProjectTo<EmailTemplateResponse>
                                        (_mapper.ConfigurationProvider)
                                    .SingleOrDefaultAsync();

        if (emailTemplate is null)
            throw new ResourceNotFoundException($"Email Template Name, {templateName}, was not found.");

        return _mapper.Map<EmailTemplateResponse>(emailTemplate);
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="model">The details for the new email template.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the EmailTemplateResponse for the newly created template.</returns>
    public async Task<EmailTemplateResponse> CreateAsync(CreateEmailTemplateRequest model)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        try {
            // Validate if the email template already created
            if (await _emailTemplates.ExistsByNameAsync(model.TemplateName))
                throw new ResourceAlreadyExistsException($"The Email Template name, '{model.TemplateName}', already exists. Please try a different Template Name.");

            // validate entire model is provided
            if (model is null) throw new ArgumentNullException(nameof(model));

            // Map model to new Email Template object and set properties
            var entity = _mapper.Map<EmailTemplate>(model);
            entity.CreatedAt = DateTime.UtcNow;

            // Save Email Template
            await _emailTemplates.AddAsync(entity);
            await _uow.SaveChangesAsync();

            await tx.CommitAsync();

            return _mapper.Map<EmailTemplateResponse>(entity);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();

            _logger.LogError(ex, "Error during Email Template creation: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Deletes an email template by its ID.
    /// </summary>
    /// <param name="id">The ID of the email template to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the DeleteResponse.</returns>
    public async Task<DeleteResponse> DeleteAsync(int id)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        try
        {
            var emailTemplate = await GetEmailTemplate(id);


            // TODO: Implement deleted data information
            /*deleteResponse.Message = "Email Template delet succesfully";
            deleteResponse.StatusCode = 200;
            deleteResponse.DeletedBy = account;
            deleteResponse.DeletedAt = DateTime.UtcNow;
            deleteResponse.ResourceId = DeleteEmailTemplateId.ToString();
            deleteResponse.IsSuccess = true;*/

            var entity = await GetRequiredAsync(id);

            _emailTemplates.Remove(entity);
            await _uow.SaveChangesAsync();

            await tx.CommitAsync();

            // If you already map DeleteResponse from entity, keep that.
            // Otherwise this is a safe explicit response:
            return new DeleteResponse
            {
                IsSuccess = true,
                Message = "Email Template deleted successfully.",
                ResourceId = id.ToString(),
                DeletedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();

            _logger.LogError(ex, "Error during Email Template deletion : {Message}", ex.Message);
            throw new DeletionFailedException($"Failed to delete Email Template with ID {id}", ex);
        }
    }

    /// <summary>
    /// Fetches all email templates.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of EmailTemplateResponses.</returns>
    public async Task<IEnumerable<EmailTemplateResponse>> GetAllAsync()
    {
        var list = await _emailTemplates.Query()
                            .AsNoTracking()
                            .OrderByDescending(x => x.CreatedAt)
                            .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
                            .ToListAsync();

        return list;
    }

    public async Task<PagedResult<EmailTemplateResponse>> GetAllAsync(
        string? name,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1) throw new AppException("Page number must be >= 1.");
        if (pageSize < 1) throw new AppException("Page size must be >= 1.");

        var q = _emailTemplates.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            q = q.Where(x => x.TemplateName == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            q = q.Where(x =>
                x.TemplateName.Contains(searchQuery) ||
                x.Subject.Contains(searchQuery) ||
                x.ContentViewPath.Contains(searchQuery));
        }

        var total = await q.CountAsync();
        if (total == 0)
            throw new ResourceNotFoundException("No email templates found.");

        var data = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<EmailTemplateResponse>
        {
            Data = data,
            Pagination = new PaginationMetadata(total, pageSize, pageNumber)
        };
    }

    /// <summary>
    /// Updates an existing email template.
    /// </summary>
    /// <param name="id">The ID of the email template to update.</param>
    /// <param name="model">The updated details for the email template.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated EmailTemplateResponse.</returns>
    public async Task<EmailTemplateResponse> UpdateAsync(int id, UpdateEmailTemplateRequest model)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));

        var entity = await GetRequiredAsync(id);

        // Uniqueness check only if template name is changing
        if (!string.IsNullOrWhiteSpace(model.TemplateName) &&
            !string.Equals(entity.TemplateName, model.TemplateName, StringComparison.Ordinal) &&
            await _emailTemplates.ExistsByNameAsync(model.TemplateName))
        {
            throw new ResourceAlreadyExistsException(
                $"The Template name, '{model.TemplateName}', already exists, please try a different template name.");
        }

        // Patch-style update: your mapping profile should ignore null/empty if you want that behavior.
        _mapper.Map(model, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        _emailTemplates.Update(entity);
        await _uow.SaveChangesAsync();

        return _mapper.Map<EmailTemplateResponse>(entity);
    }

    public async Task<string> RenderEmailTemplateAsync(string templateName, Dictionary<string, string> model)
    {
        if (string.IsNullOrWhiteSpace(templateName))
            throw new AppException("Template name is required.");

        templateName = templateName.Trim();

        var exists = await _emailTemplates.ExistsByNameAsync(templateName);
        if (!exists)
            throw new AppException($"Email template '{templateName}' not found.");

        model ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!model.ContainsKey("currentYear"))
            model["currentYear"] = DateTime.UtcNow.Year.ToString();

        var viewPath = _emailTemplateResolver.GetTemplatePath(templateName);

        try
        {
            // First try: render normally
            var html = await _razorRenderer.RenderViewToStringAsync(viewPath, model);

            // Optional: support {{token}} syntax without changing cshtml
            html = ReplaceDoubleCurlyTokens(html, model);

            return html;
        }
        catch (InvalidOperationException ex)
            when (ex.Message.Contains("RenderBody invocation", StringComparison.OrdinalIgnoreCase))
        {
            // This template is a LAYOUT (contains @RenderBody()).
            // Render it as a layout by rendering a host view that sets Layout = viewPath.
            var wrapperModel = new Dictionary<string, string>(model, StringComparer.OrdinalIgnoreCase)
            {
                ["__layout"] = viewPath
            };

            const string hostView = "~/Views/EmailTemplates/_EmailLayoutHost.cshtml";

            var html = await _razorRenderer.RenderViewToStringAsync(hostView, wrapperModel);

            // Optional: support {{token}} syntax without changing cshtml
            html = ReplaceDoubleCurlyTokens(html, model);

            return html;
        }
    }

    private static string ReplaceDoubleCurlyTokens(string html, IDictionary<string, string> model)
    {
        if (string.IsNullOrEmpty(html) || model is null) return html;

        foreach (var kv in model)
            html = html.Replace("{{" + kv.Key + "}}", kv.Value ?? string.Empty);

        return html;
    }

    /// //////////////////
    /// Helper Methods ///
    /// //////////////////

    /// <summary>
    /// Helper method to fetch an email template by its ID.
    /// </summary>
    /// <param name="id">The ID of the email template to fetch.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the EmailTemplate.</returns>
    private async Task<EmailTemplate> GetEmailTemplate(int id)
    {
        //var emailTemplate = await _context.EmailTemplates.FindAsync(id);

        var emailTemplate = await _emailTemplates.GetByIdAsync(id);

        if (emailTemplate == null) throw new ResourceNotFoundException($"Email Template ID, {id}, not found");

        return emailTemplate;
    }

    private async Task<EmailTemplate> GetRequiredAsync(int id)
    {
        var entity = await _emailTemplates.GetByIdAsync(id);
        if (entity is null)
            throw new ResourceNotFoundException($"Email Template ID, {id}, not found.");
        return entity;
    }
}
