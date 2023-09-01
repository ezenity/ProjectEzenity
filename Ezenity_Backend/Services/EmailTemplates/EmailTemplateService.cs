using AutoMapper;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models.Common.EmailTemplates;
using Ezenity_Backend.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ezenity_Backend.Services.EmailTemplates
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly TokenHelper _tokenHelper;

        public EmailTemplateService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings, ILogger<EmailTemplateService> logger, TokenHelper tokenHelper)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
            _tokenHelper = tokenHelper;
        }

        public IEmailTemplateResponse GetById(int id)
        {
            var emailTemplate = GetEmailTemplate(id);
            return _mapper.Map<IEmailTemplateResponse>(emailTemplate);
        }

        public IEmailTemplateResponse Create(ICreateEmailTemplateRequest model)
        {
            // Validate
            if (_context.EmailTemplates.Any(x => x.TemplateName == model.TemplateName))
                throw new AppException($"The Email Template name, '{model.TemplateName}', already exist. Please try a different Template Name.");

            // Map model to new email template object
            var emailTemplate = _mapper.Map<IEmailTemplate>(model);

            emailTemplate.CreatedAt = DateTime.UtcNow;

            // save to database
            _context.EmailTemplates.Add(emailTemplate);
            _context.SaveChanges();

            return _mapper.Map<IEmailTemplateResponse>(emailTemplate);
        }

        public void Delete(int id)
        {
            var emailTemplate = GetEmailTemplate(id);
            _context.EmailTemplates.Remove(emailTemplate);
            _context.SaveChanges();
        }

        public IEnumerable<IEmailTemplateResponse> GetAll()
        {
            var emailTemplate = _context.EmailTemplates.ToList();
            return _mapper.Map<IList<IEmailTemplateResponse>>(emailTemplate);
        }

        public IEmailTemplateResponse Update(int id, IUpdateEmailTemplateRequest model)
        {
            var emailTemplate = GetEmailTemplate(id);

            // Validate
            if (emailTemplate.TemplateName != model.TemplateName && _context.EmailTemplates.Any(x => x.TemplateName == model.TemplateName))
                throw new AppException($"The Template name, '{model.TemplateName}', already exist, please try a different template name.");

            // update commin props
            _mapper.Map(model, emailTemplate);

            emailTemplate.UpdatedAt = DateTime.UtcNow;

            _context.EmailTemplates.Update(emailTemplate);
            _context.SaveChanges();

            return _mapper.Map<IEmailTemplateResponse>(emailTemplate);
        }


        /// //////////////////
        /// Helper Methods ///
        /// //////////////////

        // Helper method to get a section by its ID
        private IEmailTemplate GetEmailTemplate(int id)
        {
            var emailTemplate = _context.EmailTemplates.Find(id);

            if (emailTemplate == null)
                throw new KeyNotFoundException("Email Template not found");

            return emailTemplate;
        }
    }
}
