using Ezenity.Domain.Entities.Files;
using Ezenity.Domain.Entities.Accounts;

namespace Ezenity.Domain.Entities.Vault;

public class VaultEmblem
{
    public int Id { get; set; }
    /// <summary>
    /// Unique stable identifier used by UI/routes (e.g., "verified-rider", "night-runner-s1").
    /// </summary>
    public string Slug { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    /// <summary>
    /// Optional grouping tag like "S1", "S2", "Halloween-2026", etc.
    /// </summary>
    public string? SeasonTag { get; set; }
    public VaultEmblemRarity Rarity { get; set; } = VaultEmblemRarity.Common;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Optional image/icon stored in your FileAssets table (recommended).
    /// </summary>
    public Guid? IconFileAssetId { get; set; }
    public FileAsset? IconFileAsset { get; set; }
    // Navigation
    public ICollection<VaultMissionEmblemReward> MissionRewards { get; set; } = new List<VaultMissionEmblemReward>();
    public ICollection<AccountEmblem> EarnedBy { get; set; } = new List<AccountEmblem>();
}
