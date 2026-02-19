namespace Ezenity.Contracts.Models.Sections;

/// <summary>
/// Represents a response containing section details.
/// </summary>
public class SectionResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the section.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the title of the section.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the type of content in the section.
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets or sets the content of the section.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets or sets the layout of the section.
    /// </summary>
    public required string Layout { get; init; }

    /// <summary>
    /// Gets or sets the date the account was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// Gets or sets the date the account was last updated. Null if never updated.
    /// </summary>
    public DateTime? Updated { get; init; }

    public bool IsActive { get; init; }
    public string? AccessLevel { get; init; }
    public int? ParentSectionId { get; init; }
    public string? MetaTags { get; init; }
}
