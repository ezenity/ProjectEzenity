using System;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.Files;

namespace Ezenity.Core.Entities.Vault;

public class VaultMissionSubmission
{
    public int Id { get; set; }

    public int VaultMissionId { get; set; }
    public VaultMission Mission { get; set; } = null!;

    // The submitting user (Account PK)
    public int? SubmittedByAccountId { get; set; } // The Account PK
    public Account? SubmittedByAccount { get; set; }

    // Where the proof lives (youtube / instagram / tiktok / facebook / etc)
    public string? Platform { get; set; }

    // Link to the proof (optional if you allow direct site upload only)
    public string? ExternalUrl { get; set; }

    public DateTime SubmittedUtc { get; set; } = DateTime.UtcNow;

    // Single source of truth for status
    public VaultMissionSubmissionStatus Status { get; set; } = VaultMissionSubmissionStatus.Pending;

    // Optional notes (creator notes or admin notes depending on how you use it)
    public string? Notes { get; set; }

    // Admin review info
    public int? ReviewedByAccountId { get; set; }
    public Account? ReviewedByAccount { get; set; }
    public DateTime? ReviewedUtc { get; set; }
    public string? ReviewNote { get; set; }

    // Attachments for proof uploaded directly to site
    // Uploaded proof media (files)
    public ICollection<VaultMissionSubmissionMedia> Media { get; set; } = new List<VaultMissionSubmissionMedia>();
}
