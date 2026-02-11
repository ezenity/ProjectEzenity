using System;
using Ezenity.Core.Entities.Accounts;

namespace Ezenity.Core.Entities.Vault;

public class VaultMissionCompletion
{
    public Guid Id { get; set; }

    public int VaultMissionId { get; set; }
    public VaultMission VaultMission { get; set; } = null!;

    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public string Status { get; set; } = "Pending";

    public int? ApprovedByAccountId { get; set; }
    public Account? ApprovedByAccount { get; set; }

    public DateTime? ApprovedUtc { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    // Optional: link to the approved submission used as proof
    public int? VaultSubmissionId { get; set; }
    public VaultMissionSubmission? Submission { get; set; }
}
