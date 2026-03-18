using System.ComponentModel.DataAnnotations;

namespace Ezenity.Contracts.Models.EmailTemplates;

/// <summary>
/// Represents the request payload for creating a new email template.
/// </summary>
public class CreateEmailTemplateRequest
{
    /// <summary>
    /// Gets or sets the name of the email template.
    /// </summary>
    [Required]
    public required string TemplateName { get; set; }

    /// <summary>
    /// Gets or sets the subject line of the email.
    /// </summary>
    [Required]
    public required string Subject { get; set; }

    /// <summary>
    /// Gets or sets the path to a Razor view.
    /// </summary>
    [Required]
    public required string ContentViewPath { get; set; }

    /// <summary>
    /// Indicates if this template is the default template.
    /// </summary>
    [Required]
    public required bool IsDefault { get; set; }

    /// <summary>
    /// Indicates if the content of the email is dynamic.
    /// </summary>
    [Required]
    public required bool IsDynamic { get; set; }

    /// <summary>
    /// Gets or sets the start date for using this template.
    /// </summary>
    [Required]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date for using this template.
    /// </summary>
    [Required]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the placeholder values in the email template.
    /// </summary>
    public Dictionary<string, string> PlaceholderValues { get; set; }
}
