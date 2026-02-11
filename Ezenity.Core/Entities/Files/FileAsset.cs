using System;

namespace Ezenity.Core.Entities.Files;

public class FileAsset
{
    // Keeping it string so URLs look clean: "9f3a...c2"
    public string Id { get; set; } = null!;
    public string OriginalName { get; set; } = null!;
    public string StoredName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }

    public string Category { get; set; } = "misc"; // vault, profile, emblem, etc
    public int? OwnerAccountId { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
