namespace Ezenity.Domain.Entities.Vault;

public enum VerificationDecision
{
    AutoApproved = 1,
    ApprovedWithFlag = 2,
    PendingManulReview = 3,
    Rejected = 4
}
