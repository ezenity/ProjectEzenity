using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Models.EmailTemplates;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Ezenity_Backend.Controllers
{
    [ApiController]
    [Route("api/emailtemplates")]
    public class EmailTemplatesController : BaseController<EmailTemplate, EmailTemplateResponse, CreateEmailTemplateRequest, UpdateEmailTemplateRequest>
    {
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailTemplatesController(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }

        public override ActionResult<EmailTemplateResponse> CreateAsync(CreateEmailTemplateRequest model)
        {
            var emailTemplate = _emailTemplateService.Create(model);
            return Ok(emailTemplate);
        }

        public override IActionResult Delete(int id)
        {
            _emailTemplateService.Delete(id);
            return Ok(new { message = "Email Template deleted successfully. " } );
        }

        public override ActionResult<IEnumerable<EmailTemplateResponse>> GetAll()
        {
            var emailTemplate = _emailTemplateService.GetAll();
            return Ok(emailTemplate);
        }

        public override ActionResult<EmailTemplateResponse> GetById(int id)
        {
            var emailTemplate = _emailTemplateService.GetById(id);

            if (emailTemplate == null)
            {
                return NotFound();
            }

            return Ok(emailTemplate);
        }

        public override ActionResult<EmailTemplateResponse> Update(int id, UpdateEmailTemplateRequest model)
        {
            var emailTemplate = _emailTemplateService.GetById(id);

            if (emailTemplate == null)
            {
                return NotFound();
            }

            var updatedEmailTemplate = _emailTemplateService.Update(id, model);

            return Ok(updatedEmailTemplate);
        }
    }
}
