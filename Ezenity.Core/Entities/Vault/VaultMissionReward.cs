using System;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.Files;

namespace Ezenity.Core.Entities.Vault;

public class VaultMissionReward
{
    public int Id { get; set; }

    public int VaultMissionId { get; set; }
    public VaultMission VaultMission { get; set; } = null!;

    /// <summary>
    /// e.g. "rep", "coins", "emblem"
    /// </summary>
    public string RewardType { get; set; } = null!;

    public int Amount { get; set; }

    /// <summary>
    /// Optional extra reward info (e.g. emblem code/name).
    /// </summary>
    public string? Meta { get; set; }
}
