using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ezenity_Backend.Entities.EmailTemplates
{
    /// <summary>
    /// Represents an email template in the system.
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Gets or sets the template ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the subject line for the email template.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the content of the email template.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template is the default one.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp of the email template.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update timestamp of the email template.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets a value indicating whether the template is currently active.
        /// </summary>
        public bool IsActive => DateTime.UtcNow >= StartDate && (!EndDate.HasValue || DateTime.UtcNow <= EndDate.Value);

        /// <summary>
        /// Gets or sets the start date for when the template is considered active.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for when the template is considered inactive.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template supports dynamic content.
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Gets or sets the placeholder values and their corresponding replacements.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public Dictionary<string, string> PlaceholderValues { get; set; }

        /// <summary>
        /// Gets or sets the JSON serialized placeholder values for the template.
        /// </summary>
        [Column(TypeName = "text")]
        public string PlaceholderValuesJson
        {
            //get => Newtonsoft.Json.JsonConvert.SerializeObject(PlaceholderValues);
            //set => PlaceholderValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
            get => PlaceholderValues == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(PlaceholderValues);
            set => PlaceholderValues = value == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        }

        /// <summary>
        /// Applies dynamic content to the template by replacing placeholders with their corresponding values.
        /// </summary>
        /// <returns>The content of the template with placeholders replaced.</returns>
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
