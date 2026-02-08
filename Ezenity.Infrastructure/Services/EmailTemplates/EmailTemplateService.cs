using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.Core.Services.Emails;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.EmailTemplates;
using Ezenity.DTOs.Models.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ezenity.Infrastructure.Services.EmailTemplates
{
    /// <summary>
    /// Provides functionality to manage Email Templates.
    /// </summary>
    public class EmailTemplateService : IEmailTemplateService
    {
        /// <summary>
        /// Provides data access to the application's data store.
        /// </summary>
        private readonly IDataContext _context;

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
        private readonly ITokenHelper _tokenHelper;

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
        public EmailTemplateService(IDataContext context, IMapper mapper, IAppSettings appSettings, ILogger<EmailTemplateService> logger, ITokenHelper tokenHelper, IAuthService authService, IRazorViewRenderer razorRenderer, IEmailTemplateResolver emailTemplateResolver)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
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
            var emailTemplate = await _context.EmailTemplates
                                    .Where(x => x.Id == id)
                                    .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
                                    .SingleOrDefaultAsync();

            if (emailTemplate == null)
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
            var emailTemplate = await _context.EmailTemplates
                                    .Where(n => n.TemplateName == templateName)
                                    .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
                                    .SingleOrDefaultAsync();

            if(emailTemplate == null)
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
            // Validate
            if (await _context.EmailTemplates.AnyAsync(x => x.TemplateName == model.TemplateName))
                throw new ResourceAlreadyExistsException($"The Email Template name, '{model.TemplateName}', already exist. Please try a different Template Name.");

            // Map model to new email template object
            var emailTemplate = _mapper.Map<EmailTemplate>(model);

            emailTemplate.CreatedAt = DateTime.UtcNow;

            // save to database
            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();

            return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        }

        /// <summary>
        /// Deletes an email template by its ID.
        /// </summary>
        /// <param name="id">The ID of the email template to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the DeleteResponse.</returns>
        public async Task<DeleteResponse> DeleteAsync(int id)
        {
            var emailTemplate = await GetEmailTemplate(id);

            // TODO: Implement deleted data information
            /*deleteResponse.Message = "Email Template delet succesfully";
            deleteResponse.StatusCode = 200;
            deleteResponse.DeletedBy = account;
            deleteResponse.DeletedAt = DateTime.UtcNow;
            deleteResponse.ResourceId = DeleteEmailTemplateId.ToString();
            deleteResponse.IsSuccess = true;*/

            _context.EmailTemplates.Remove(emailTemplate);
            _context.SaveChanges();

            return _mapper.Map<DeleteResponse>(emailTemplate);
        }

        /// <summary>
        /// Fetches all email templates.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of EmailTemplateResponses.</returns>
        public async Task<IEnumerable<EmailTemplateResponse>> GetAllAsync()
        {
            var emailTemplate = await _context.EmailTemplates
                                    .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
                                    .ToListAsync();

            return _mapper.Map<IList<EmailTemplateResponse>>(emailTemplate);
        }

        public async Task<PagedResult<EmailTemplateResponse>> GetAllAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing email template.
        /// </summary>
        /// <param name="id">The ID of the email template to update.</param>
        /// <param name="model">The updated details for the email template.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated EmailTemplateResponse.</returns>
        public async Task<EmailTemplateResponse> UpdateAsync(int id, UpdateEmailTemplateRequest model)
        {
            var emailTemplate = await GetEmailTemplate(id);

            // Validate
            if (emailTemplate.TemplateName != model.TemplateName && _context.EmailTemplates.Any(x => x.TemplateName == model.TemplateName))
                throw new ResourceAlreadyExistsException($"The Template name, '{model.TemplateName}', already exist, please try a different template name.");

            // update commin props
            _mapper.Map(model, emailTemplate);

            emailTemplate.UpdatedAt = DateTime.UtcNow;

            _context.EmailTemplates.Update(emailTemplate);
            await _context.SaveChangesAsync();

            return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        }

        public async Task<string> RenderEmailTemplateAsync(string templateName, Dictionary<string, string> model)
        {
            var emailTemplateExists = await _context.EmailTemplates.AnyAsync(t => t.TemplateName == templateName);
            if (!emailTemplateExists)
                throw new AppException($"Email template '{templateName}' not found.");

            model ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Optional: make sure year exists (since your template uses it)
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
            if (string.IsNullOrEmpty(html) || model == null) return html;

            foreach (var kv in model)
            {
                html = html.Replace("{{" + kv.Key + "}}", kv.Value ?? string.Empty);
            }

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
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);

            if (emailTemplate == null) throw new ResourceNotFoundException($"Email Template ID, {id}, not found");

            return emailTemplate;
        }
    }
}
