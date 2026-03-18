using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Domain.Entities.Vault;

public class MissionProofReview
{
    public Guid Id { get; set; }

    public Guid MissionSessionId { get; set; }
    public MissionSession MissionSession { get; set; } = null;

    public MissionProofReviewType ReviewType { get; set; }
    public MissionProofReviewStatus status { get; set; } = MissionProofReviewStatus.Pending;

    public Guid? AssignedReviewerUserId { get; set; }
    public Guid? ReviewByUserId { get; set; }

    public string? ReviewerNotes { get; set; }
    public string? RejectionReason { get; set; }

    public DateTime? ReviewedUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
}
