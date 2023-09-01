using Ezenity_Backend.Entities.Common;
using System.Collections.Generic;

namespace Ezenity_Backend.Entities.Emails
{
    public class EmailMessage : IEmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string> DynamicValues { get; set; }
        public string From { get; set; }
    }
}
