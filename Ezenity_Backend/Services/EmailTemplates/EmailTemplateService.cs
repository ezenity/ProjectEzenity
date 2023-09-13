using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Helpers.Exceptions;
using Ezenity_Backend.Models;
using Ezenity_Backend.Models.EmailTemplates;
using Ezenity_Backend.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity_Backend.Services.EmailTemplates
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<IEmailTemplateService> _logger;
        private readonly TokenHelper _tokenHelper;
        private readonly IAuthService _authService;

        public EmailTemplateService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings, ILogger<IEmailTemplateService> logger, TokenHelper tokenHelper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
            _tokenHelper = tokenHelper;
            _authService = authService;
        }

        public async Task<EmailTemplateResponse> GetByIdAsync(int id)
        {
            //var emailTemplate = await GetEmailTemplate(id);
            var emailTemplate = await _context.EmailTemplates
                                    .Where(x => x.Id == id)
                                    .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
                                    .SingleOrDefaultAsync();

            if (emailTemplate == null)
                throw new ResourceNotFoundException($"Email Template ID, {id}, was not found.");

            return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        }

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

        public async Task<DeleteResponse> DeleteAsync(int id)
        {
            var emailTemplate = await GetEmailTemplate(id);

            _context.EmailTemplates.Remove(emailTemplate);
            _context.SaveChanges();

            return _mapper.Map<DeleteResponse>(emailTemplate);
        }

        public async Task<IEnumerable<EmailTemplateResponse>> GetAllAsync()
        {
            var emailTemplate = await _context.EmailTemplates
                                    .ProjectTo<EmailTemplateResponse>(_mapper.ConfigurationProvider)
                                    .ToListAsync();

            return _mapper.Map<IList<EmailTemplateResponse>>(emailTemplate);
        }

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


        /// //////////////////
        /// Helper Methods ///
        /// //////////////////

        // Helper method to get a section by its ID
        private async Task<EmailTemplate> GetEmailTemplate(int id)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);

            if (emailTemplate == null) throw new ResourceNotFoundException($"Email Template ID, {id}, not found");

            return emailTemplate;
        }
    }
}
