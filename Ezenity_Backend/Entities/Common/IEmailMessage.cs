using System.Collections.Generic;

namespace Ezenity_Backend.Entities.Common
{
    public interface IEmailMessage
    {
        string To { get; set; }
        string Subject { get; set; }
        string TemplateName { get; set; }
        Dictionary<string, string> DynamicValues { get; set; }
        string From { get; set; }
    }
}
