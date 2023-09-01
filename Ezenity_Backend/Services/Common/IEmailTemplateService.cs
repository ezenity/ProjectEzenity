using Ezenity_Backend.Entities;
using Ezenity_Backend.Models.Common.EmailTemplates;

namespace Ezenity_Backend.Services.Common
{
    public interface IEmailTemplateService : IBaseService<IEmailTemplate, IEmailTemplateResponse, ICreateEmailTemplateRequest, IUpdateEmailTemplateRequest>
    {
    }
}
