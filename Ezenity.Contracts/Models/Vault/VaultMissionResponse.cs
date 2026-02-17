namespace Ezenity.Contracts.Models.Vault;

public class VaultMissionResponse
{
    public int Id { get; set; }
    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int RepReward { get; set; }
    public int CoinReward { get; set; }
    public string? EmblemName { get; set; }

    // optional: including user progress for UI badges
    public string? MyStatus { get; set; } // "Pending/Approved/Rejected"
}
