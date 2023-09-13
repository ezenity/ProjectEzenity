using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models.EmailTemplates
{
    public class CreateEmailTemplateRequest
    {
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDynamic { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Dictionary<string, string> PlaceholderValues { get; set; }
    }
}
