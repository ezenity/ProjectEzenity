using System;

namespace Ezenity_Backend.Models.EmailTemplates
{
    public class EmailTemplateResponse
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDynamic { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
