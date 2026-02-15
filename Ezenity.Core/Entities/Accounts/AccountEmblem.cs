using Ezenity.Domain.Entities.Vault;

namespace Ezenity.Domain.Entities.Accounts;

/// <summary>
/// Join entity: which accounts have earned which emblems.
/// </summary>
public class AccountEmblem
{
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public int VaultEmblemId { get; set; }
    public VaultEmblem Emblem { get; set; } = null!;

    /// <summary>
    /// Optional: record which mission granted it (if applicable).
    /// </summary>
    public int? ObtainedFromVaultMissionId { get; set; }
    public VaultMission? ObtainedFromVaultMission { get; set; }

    public DateTime ObtainedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional: admin/internal note (e.g., "manual grant", "season migration").
    /// </summary>
    public string? Note { get; set; }
}
