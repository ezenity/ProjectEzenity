using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models.Emails
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
        public IDictionary<string, string> PlaceholderValues { get; set; }
    }
}
