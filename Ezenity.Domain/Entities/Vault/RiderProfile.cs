namespace Ezenity.Domain.Entities.Vault;

public class RiderProfile
{
    public Guid UserId { get; set; }

    public string RiderAlias { get; set; } = null!;
    public int TierLevel { get; set; } = 0;
    public int Rep { get; set; } = 0;
    public int Coins { get; set; } = 0;

    public bool IsVerified { get; set; }
    public bool IsSuspended { get; set; }

    public Guid HomeChapterId { get; set; }

    public int TotalMissionsCompleted { get; set; }
    public int CurrentCompletionStreak { get; set; }
    public int LongestCompletionStreak { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
