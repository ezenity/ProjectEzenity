using AutoMapper;
using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Helpers;
using Ezenity_Backend.Models.Emails;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ezenity_Backend.Services.Emails
{
    public class EmailTemplateService : BaseService<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest>, IEmailTemplateService
    {
        public EmailTemplateService(DataContext context, IMapper mapper, IOptions<AppSettings> appSettings)
            : base(context, mapper, appSettings)
        {
        }

        public override EmailTemplateResponse Create(CreateEmailTemplateRequest model)
        {
            // Validate
            if (_context.EmailTemplates.Any(x => x.TemplateName == model.TemplateName))
                throw new AppException($"The Email Template name, '{model.TemplateName}', already exist. Please try a different Template Name.");

            // Map model to new email template object
            var emailTemplate = _mapper.Map<EmailTemplate>(model);

            emailTemplate.CreatedAt = DateTime.UtcNow;

            // save to database
            _context.EmailTemplates.Add(emailTemplate);
            _context.SaveChanges();

            return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        }

        public override void Delete(int id)
        {
            var emailTemplate = GetEmailTemplate(id);
            _context.EmailTemplates.Remove(emailTemplate);
            _context.SaveChanges();
        }

        public override IEnumerable<EmailTemplateResponse> GetAll()
        {
            var emailTemplate = _context.EmailTemplates.ToList();
            return _mapper.Map<IList<EmailTemplateResponse>>(emailTemplate);
        }

        public override EmailTemplateResponse GetById(int id)
        {
            var emailTemplate = GetEmailTemplate(id);
            return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        }

        public override EmailTemplateResponse Update(int id, UpdateEmailTemplateRequest model)
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

            return _mapper.Map<EmailTemplateResponse>(emailTemplate);
        }

        // Helper method to get a section by its ID
        private EmailTemplate GetEmailTemplate(int id)
        {
            var emailTemplate = _context.EmailTemplates.Find(id);

            if (emailTemplate == null)
                throw new KeyNotFoundException("Email Template not found");

            return emailTemplate;
        }
    }
}
