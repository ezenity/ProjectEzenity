using System;

namespace Ezenity.Core.Entities.Vault;

public class VaultMission
{
    public int Id { get; set; }
    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Description { get; set; } = null!; // markdown or plain text

    public int RepReward { get; set; }
    public int CoinReward { get; set; }

    public int? EmblemId { get; set; }
    public VaultEmblem? Emblem { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
