namespace Ezenity.Domain.Entities.Vault;

public enum MissionSessionStatus
{
    Activated = 1,
    WaitingForStartZone = 2,
    InProgress = 3,
    Completed = 4,
    PendingReview = 5,
    Rejected = 6,
    Dnf = 7,
    Cancelled = 8
}
