namespace Ezenity.Contracts.Models.Files;

/// <summary>
/// Public response for a stored file item.
/// </summary>
public class FileItemResponse
{
    public string Id { get; set; } = default!;           // GUID (N format)
    public string OriginalName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long Size { get; set; }
    public DateTime CreatedUtc { get; set; }

    // Helpful for frontend to render directly
    // URL to retrieve the file (controller fills this).
    public string Url { get; set; }
    public string? Scope { get; set; }       // "profile" | "vault" | "emblem" etc.
}
