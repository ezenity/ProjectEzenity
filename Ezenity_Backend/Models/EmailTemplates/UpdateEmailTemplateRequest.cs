using Ezenity_Backend.Models.Common.EmailTemplates;
using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models.EmailTemplates
{
    public class UpdateEmailTemplateRequest : IUpdateEmailTemplateRequest
    {
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDynamic { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IDictionary<string, string> PlaceholderValues { get; set; }
    }
}
