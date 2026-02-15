namespace Ezenity.Domain.Entities.Vault;

public class VaultMission
{
    public int Id { get; set; }
    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!; // markdown or plain text
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    // one-to-one numeric rewards
    public VaultMissionReward Reward { get; set; } = new VaultMissionReward();

    // emblem rewards (many-to-many via join)
    public ICollection<VaultMissionEmblemReward> EmblemRewards { get; set; } = new List<VaultMissionEmblemReward>();

    // submissions/comments/completions
    public ICollection<VaultMissionSubmission> Submissions { get; set; } = new List<VaultMissionSubmission>();
    public ICollection<VaultMissionComment> Comments { get; set; } = new List<VaultMissionComment>();
    public ICollection<VaultMissionCompletion> Completions { get; set; } = new List<VaultMissionCompletion>();
}
