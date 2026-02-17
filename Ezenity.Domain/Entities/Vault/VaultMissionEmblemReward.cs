namespace Ezenity.Domain.Entities.Vault;

/// <summary>
/// Join entity: which emblems are rewarded by which mission.
/// </summary>
public class VaultMissionEmblemReward
{
    public int Id { get; set; }
    public int VaultMissionId { get; set; }
    public VaultMission Mission { get; set; } = null!;
    public int VaultEmblemId { get; set; }
    public VaultEmblem Emblem { get; set; } = null!;
    /// <summary>
    /// Usually 1. Keeping it allows future stacking logic if we ever want it.
    /// </summary>
    public int Quantity { get; set; } = 1;
    /// <summary>
    /// Optional: highlight a "main" emblem reward for a mission.
    /// </summary>
    public bool IsPrimary { get; set; } = false;
}
