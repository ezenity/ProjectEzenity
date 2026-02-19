using System.ComponentModel.DataAnnotations;

namespace Ezenity.Contracts.Models.Sections;

/// <summary>
/// Represents a request to create a new section.
/// </summary>
public sealed class CreateSectionWithAdditonalRequest : CreateSectionRequest
{
    /// <summary>
    /// Gets or sets who can see the section
    /// </summary>
    [Required(ErrorMessage = "AccessLevel is required.")]
    [MaxLength(50)]
    public string AccessLevel { get; init; } = default!;

    /// <summary>
    /// Get or set the SEO meta tags that could be applied to the section
    /// </summary>
    [MaxLength(2000)]
    public string? MetaTags { get; init; }
}


