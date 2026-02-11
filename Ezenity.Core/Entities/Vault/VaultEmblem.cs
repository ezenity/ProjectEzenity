using System;

namespace Ezenity.Core.Entities.Vault;

public class VaultEmblem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Rarity { get; set; } = "Common";

    // points at a file record (image)
    public string? ImageFileId { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
