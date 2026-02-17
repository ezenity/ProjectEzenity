using System.ComponentModel.DataAnnotations;

namespace Ezenity.Contracts.Models.Sections;

/// <summary>
/// Represents a request to create a new section.
/// </summary>
public class CreateSectionWithAdditonalRequest : CreateSectionRequest
{
    /// <summary>
    /// Gets or sets who can see the section
    /// </summary>
    [Required]
    public string AccessLevel { get; set; }

    /// <summary>
    /// Get or set the SEO meta tags that could be applied to the section
    /// </summary>
    public string MetaTags { get; set; }
}
