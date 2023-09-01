using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models.Common.EmailTemplates
{
    public interface ICreateEmailTemplateRequest
    {
        string TemplateName { get; set; }
        string Subject { get; set; }
        string Content { get; set; }
        bool IsDefault { get; set; }
        bool IsDynamic { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        IDictionary<string, string> PlaceholderValues { get; set; }
    }
}
