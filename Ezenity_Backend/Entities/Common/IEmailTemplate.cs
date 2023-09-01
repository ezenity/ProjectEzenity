using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Entities.Common
{
    public interface IEmailTemplate
    {
        int Id { get; set; }
        string TemplateName { get; set; }
        string Subject { get; set; }
        string Content { get; set; }
        bool IsDefault { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        bool IsActive { get; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        bool IsDynamic { get; set; }
        IDictionary<string, string> PlaceholderValues { get; set; }
        string PlaceholderValuesJson { get; set; }

        // Method signatures
        string ApplyDynamicContent();
    }
}
