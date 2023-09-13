using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ezenity_Backend.Entities.EmailTemplates
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive => DateTime.UtcNow >= StartDate && (!EndDate.HasValue || DateTime.UtcNow <= EndDate.Value);

        // Optional date range to specify when the tempalte is active
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // If true, this template allows dynamic content substitution using placeholders
        public bool IsDynamic { get; set; }

        // Placeholder values and their corresponding replacements
        [NotMapped] // this property will not be mapped to the database
        public Dictionary<string, string> PlaceholderValues { get; set; }

        // This property will be persisted as JSON string in the database
        [Column(TypeName = "text")]
        public string PlaceholderValuesJson
        {
            //get => Newtonsoft.Json.JsonConvert.SerializeObject(PlaceholderValues);
            //set => PlaceholderValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
            get => PlaceholderValues == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(PlaceholderValues);
            set => PlaceholderValues = value == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        }

        // Apply dynamic content to the template
        public string ApplyDynamicContent()
        {
            if (!IsDynamic || string.IsNullOrEmpty(Content) || PlaceholderValues.Values == null || PlaceholderValues.Count == 0 || PlaceholderValues.Values.Count == 0)
                return Content;

            // Replace placeholders with their corresponding values
            var result = Content;
            foreach (var (placeholder, value) in PlaceholderValues)
            {
                result = result.Replace(placeholder, value);
            }

            return result;
        }
    }
}
