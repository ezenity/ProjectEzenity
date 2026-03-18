namespace Ezenity.Domain.Entities.Vault;

public class UserMissionProgress
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid MissionId { get; set; }

    public bool IsUnlocked { get; set; }
    public bool IsCompleted { get; set; }

    public int Attempts { get; set; }
    public DateTime? FirstUnlockedUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }

    public Guid? LastMissionSessionId { get; set; }
}
