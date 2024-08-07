﻿using System;

namespace Ezenity.DTOs.Models.EmailTemplates
{
    /// <summary>
    /// Represents the response payload containing details of an email template.
    /// </summary>
    public class EmailTemplateResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the email template.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the email template.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the subject line of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the path to a Razor view.
        /// </summary>
        public string ContentViewPath { get; set; }

        /// <summary>
        /// Indicates if this template is the default template.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Indicates if the content of the email is dynamic.
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Gets or sets the start date for using this template.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for using this template.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Indicates if the email template is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the placeholder values in the email template.
        /// </summary>
        public Dictionary<string, string> PlaceholderValues { get; set; }
    }
}
